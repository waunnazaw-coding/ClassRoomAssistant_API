using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class AssignmentSubmissionService : IAssignmentSubmissionService
{
    private readonly IAssignmentSubmissionRepository _submissionRepo;
    private readonly ISubmissionResponseRepository _responseRepo;
    private readonly DbContextClassName _context;

    public AssignmentSubmissionService(
        IAssignmentSubmissionRepository submissionRepo,
        ISubmissionResponseRepository responseRepo,
        DbContextClassName context)
    {
        _submissionRepo = submissionRepo;
        _responseRepo = responseRepo;
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
                ResponseType = r.ResponseType,
                FilePath = r.FilePath,
                FileName = r.FileName,
                FileSize = r.FileSize,
                MimeType = r.MimeType,
                Link = r.Link,
                UploadedAt = r.UploadedAt
            }).ToList()
        }).ToList();
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
        response.FileName = dto.FileName ?? response.FileName;
        response.FileSize = dto.FileSize ?? response.FileSize;
        response.MimeType = dto.MimeType ?? response.MimeType;
        response.Link = dto.Link ?? response.Link;

        await _responseRepo.UpdateAsync(response);

        return new SubmissionResponseDto
        {
            Id = response.Id,
            ResponseType = response.ResponseType,
            FilePath = response.FilePath,
            FileName = response.FileName,
            FileSize = response.FileSize,
            MimeType = response.MimeType,
            Link = response.Link,
            UploadedAt = response.UploadedAt
        };
    }

    public async Task<AssignmentSubmissionDto> CreateSubmissionAsync(AssignmentSubmissionCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var submission = new AssignmentSubmission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            SubmittedAt = DateTime.UtcNow
        };

        // Insert AssignmentSubmission
        await _submissionRepo.AddAsync(submission);

        // Map SubmissionResponses and assign SubmissionId
        var responses = dto.Responses.Select(r => new SubmissionResponse
        {
            SubmissionId = submission.Id,
            ResponseType = r.ResponseType,
            FilePath = r.FilePath,
            FileName = r.FileName,
            FileSize = r.FileSize,
            MimeType = r.MimeType,
            Link = r.Link,
            UploadedAt = DateTime.UtcNow
        }).ToList();

        // Insert SubmissionResponses in bulk
        await _responseRepo.AddRangeAsync(responses);

        await transaction.CommitAsync();

        // Prepare response DTO
        return new AssignmentSubmissionDto
        {
            Id = submission.Id,
            AssignmentId = submission.AssignmentId,
            StudentId = submission.StudentId,
            SubmittedAt = submission.SubmittedAt,
            SubmissionResponses = responses.Select(r => new SubmissionResponseDto
            {
                Id = r.Id,
                ResponseType = r.ResponseType,
                FilePath = r.FilePath,
                FileName = r.FileName,
                FileSize = r.FileSize,
                MimeType = r.MimeType,
                Link = r.Link,
                UploadedAt = r.UploadedAt
            }).ToList()
        };
    }
}
