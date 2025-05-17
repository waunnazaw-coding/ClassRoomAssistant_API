using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;
using System.Net;
using FluentFTP;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class AssignmentSubmissionService : IAssignmentSubmissionService
    {
        
        private readonly string _ftpHost = "127.0.0.1";
        private readonly string _ftpUser = "ftptest";
        private readonly string _ftpPass = "179239";
        private readonly IAssignmentSubmissionRepository _submissionRepo;
        private readonly ISubmissionResponseRepository _responseRepo;
        private readonly MegaStorageService _megaStorage;
        private readonly DbContextClassName _context;

        public AssignmentSubmissionService(
            IAssignmentSubmissionRepository submissionRepo,
            ISubmissionResponseRepository responseRepo,
            MegaStorageService megaStorage,
            DbContextClassName context)
        {
            _submissionRepo = submissionRepo;
            _responseRepo = responseRepo;
            _megaStorage = megaStorage;
            _context = context;
        }

        public async Task<List<AssignmentSubmissionDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            var submissions = await _submissionRepo.GetSubmissionsByAssignmentIdAsync(assignmentId);

            return submissions.Select(s => new AssignmentSubmissionDto
            {
                Id = s.Id,
                AssignmentId = s.AssignmentId,
                StudentId = s.StudentId,
                SubmittedAt = s.SubmittedAt,
                SubmissionResponses = s.SubmissionResponses.Select(r => new SubmissionResponseDto
                {
                    Id = r.Id,
                    ResponseType = r.FileType,
                    Link = r.Link,
                    UploadedAt = r.UploadedAt
                }).ToList()
            }).ToList();
        }

         public async Task<AssignmentSubmissionDto> CreateSubmissionAsync(AssignmentSubmissionCreateDto dto, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var submission = new AssignmentSubmission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = dto.StudentId,
                SubmittedAt = DateTime.UtcNow
            };

            // Insert AssignmentSubmission
            await _submissionRepo.AddAsync(submission);

            string? ftpFilePath = null;
            string? fileUrl = null;

            if (dto.ResponseType == "File" && dto.File != null)
            {
                // Define remote directory structure: e.g. /assignments/{assignmentId}/submissions/{studentId}
                string remoteDir = $"/assignments/{dto.AssignmentId}/submissions/{dto.StudentId}";


                ftpFilePath = await UploadFileToFtpAsync(dto.File, remoteDir, cancellationToken);

                // Construct FTP URL (adjust based on your FTP server setup)
                //fileUrl = $"ftp://{_ftpHost}{ftpFilePath}";
            }
            else if (dto.ResponseType == "Link")
            {
                fileUrl = dto.Link;
            }

            var response = new SubmissionResponse
            {
                SubmissionId = submission.Id,
                FileType = dto.ResponseType,
                FilePath = ftpFilePath,
                Link = fileUrl,
                FileName = dto.File.FileName,
                UploadedAt = DateTime.UtcNow
            };

            // Insert SubmissionResponse
            await _responseRepo.AddAsync(response);

            await transaction.CommitAsync(cancellationToken);

            // Prepare response DTO
            return new AssignmentSubmissionDto
            {
                Id = submission.Id,
                AssignmentId = submission.AssignmentId,
                StudentId = submission.StudentId,
                SubmittedAt = submission.SubmittedAt,
                
            };
        }
        
        private async Task<string> UploadFileToFtpAsync(IFormFile file, string remoteDir, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Generate unique filename using Guid
            string remoteFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string remoteFilePath = $"{remoteDir.TrimEnd('/')}/{remoteFileName}";

            // Save IFormFile to temp file
            var tempFilePath = Path.GetTempFileName();

            try
            {
                using (var tempStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(tempStream, cancellationToken);
                }

                using var ftp = new AsyncFtpClient(_ftpHost, new NetworkCredential(_ftpUser, _ftpPass));
                ftp.Config.EncryptionMode = FtpEncryptionMode.Explicit; // or None if no TLS
                ftp.Config.ValidateAnyCertificate = true; // for self-signed certs
                ftp.Config.DataConnectionType = FtpDataConnectionType.PASV;

                ftp.Config.ConnectTimeout = 30000;
                ftp.Config.ReadTimeout = 60000;
                ftp.Config.DataConnectionReadTimeout = 60000;

                await ftp.Connect(cancellationToken);

                // Ensure remote directory exists
                string ftpRemoteDir = Path.GetDirectoryName(remoteFilePath).Replace('\\', '/');
                if (!await ftp.DirectoryExists(ftpRemoteDir, cancellationToken))
                {
                    await ftp.CreateDirectory(ftpRemoteDir, true, cancellationToken);
                }

                // Upload file with overwrite and no verification to avoid checksum issues
                var result = await ftp.UploadFile(
                    tempFilePath,
                    remoteFilePath,
                    FtpRemoteExists.Overwrite,
                    createRemoteDir: false,
                    verifyOptions: FtpVerify.None,
                    progress: null,
                    token: cancellationToken);

                if (result != FtpStatus.Success)
                    throw new Exception("FTP upload failed");

                await ftp.Disconnect();


                if (result != FtpStatus.Success)
                    throw new Exception("FTP upload failed");

                return remoteFilePath;
            }
            finally
            {
                // Delete temp file
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }


        public async Task<Stream> DownloadFileFromFtpAsync(string remoteFilePath, CancellationToken cancellationToken)
        {
            var ftp = new AsyncFtpClient(_ftpHost, new NetworkCredential(_ftpUser, _ftpPass));
            ftp.Config.EncryptionMode = FtpEncryptionMode.Explicit; // or None if no TLS
            ftp.Config.ValidateAnyCertificate = true; // for self-signed certs
            ftp.Config.DataConnectionType = FtpDataConnectionType.PASV;

            ftp.Config.ConnectTimeout = 30000;
            ftp.Config.ReadTimeout = 60000;
            ftp.Config.DataConnectionReadTimeout = 60000;

            await ftp.Connect(cancellationToken);

            // Check if file exists
            //if (!await ftp.FileExists(remoteFilePath, cancellationToken))
            //{
            //    await ftp.Disconnect();
            //    throw new FileNotFoundException($"File not found on FTP server: {remoteFilePath}");
            //}

            // Download file into a MemoryStream
            var memoryStream = new MemoryStream();
            var downloadSuccess = await ftp.DownloadStream(memoryStream, remoteFilePath, token: cancellationToken);

            await ftp.Disconnect();

            if (!downloadSuccess)
            {
                throw new Exception("Failed to download file from FTP server.");
            }

            memoryStream.Position = 0; // Reset stream position before returning
            return memoryStream;
        }



        public async Task<SubmissionResponseDto> UpdateResponseAsync(
            int assignmentId, 
            int submissionId, 
            int responseId, 
            SubmissionResponseUpdateDto dto
        )
        {
            var response = await _responseRepo.GetByIdWithSubmissionAsync(responseId);
        
            // Validate ownership
            if (response?.Submission == null || 
                response.Submission.AssignmentId != assignmentId || 
                response.Submission.Id != submissionId)
            {
                throw new KeyNotFoundException("Response not found or access denied");
            }

            // Update fields (null-coalescing to preserve existing values if not provided)
            response.FilePath = dto.FilePath ?? response.FilePath;
            response.Link = dto.Link ?? response.Link;

            await _responseRepo.UpdateAsync(response);

            return new SubmissionResponseDto
            {
                Id = response.Id,
                ResponseType = response.FileType,
                FilePath = response.FilePath,
                Link = response.Link,
                UploadedAt = response.UploadedAt
            };
        }

    }
}
