using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class MaterialService : IMaterialService
{
    private readonly IClassWorkRepository _classWorkRepo;
    private readonly IMaterialRepository _materialRepo;
    private readonly IAttachmentRepository _attachmentRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IClassParticipantsRepository _classParticipantsRepo;
    private readonly DbContextClassName _context;

    public MaterialService(
        IClassWorkRepository classWorkRepo,
        IMaterialRepository materialRepo,
        IAttachmentRepository attachmentRepo,
        INotificationRepository notificationRepo,
        IClassParticipantsRepository classParticipantsRepo,
        DbContextClassName context)
    {
        _classWorkRepo = classWorkRepo;
        _materialRepo = materialRepo;
        _attachmentRepo = attachmentRepo;
        _context = context;
    }

    public async Task<MaterialDetailResponseDto?> GetMaterialDetailsAsync(int materialId)
    {
        var material = await _materialRepo.GetByIdWithAttachmentsAsync(materialId);
        if (material == null) return null;
        
        var attachments = await _attachmentRepo.GetByReferenceAsync(material.Id, "Material");

        return new MaterialDetailResponseDto
        {
            MaterialId = material.Id,
            Title = material.Title,
            Description = material.Description,
            ClassWorkId = material.ClassWorkId,
            Attachments = attachments
                .OrderBy(a => a.Id)
                .Select(a => new AttachmentDetailDto
                {
                    Id = a.Id,
                    FileType = a.FileType ?? string.Empty,
                    FileUrl = a.FileUrl,
                    FilePath = a.FilePath,
                    CreatedBy = a.CreatedBy,
                    CreatedAt = a.CreatedAt
                })
                .ToList()
        };
    }
    public async Task<MaterialResponseDto> CreateMaterialWithAttachmentsAsync(MaterialCreateRequestDto request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // 1. Create ClassWork
            var classWork = await _classWorkRepo.AddClassWorkAsync(new ClassWork
            {
                ClassId = request.ClassId,
                TopicId = request.TopicId,
                Type = "Material",
                CreatedAt = DateTime.UtcNow
            });

            // 2. Create Material
            var material = await _materialRepo.AddAsync(new Material
            {
                Title = request.Title,
                ClassWorkId = classWork.Id,
                CreatedAt = DateTime.UtcNow
            });

            // 3. Create Attachments
            var attachments = request.Attachments.Select(a => new Attachment
            {
                ReferenceId = material.Id,
                ReferenceType = "Material",
                FileType = a.FileType,
                FilePath = a.FilePath,
                FileUrl = a.FileUrl,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _attachmentRepo.AddRangeAsync(attachments);
            
            var studentUserIds = await _classParticipantsRepo.GetStudentUserIdsByClassIdAsync(request.ClassId);
            
            var notifications = studentUserIds.Select(userId => new Notification
            {
                UserId = userId,
                Type = "Materail",
                ReferenceId = material.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _notificationRepo.AddRangeAsync(notifications);

            await transaction.CommitAsync();

            return new MaterialResponseDto
            {
                MaterialId = material.Id,
                Title = material.Title,
                ClassWorkId = classWork.Id,
                Attachments = attachments.Select(a => new AttachmentResponseDto
                {
                    Id = a.Id,
                    FileType = a.FileType,
                    FileUrl = a.FileUrl,
                    FilePath = a.FilePath,
                }).ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<MaterialResponseDto> UpdateMaterialWithAttachmentsAsync(int materialId, int userId, MaterialUpdateRequestDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var material = await _materialRepo.GetByIdAsync(materialId);
            if (material == null) throw new KeyNotFoundException("Material not found.");

            material.Title = dto.Title;
            material.Description = dto.Description;
            await _materialRepo.UpdateAsync(material);
            
            var existingAttachments = await _attachmentRepo.GetByReferenceAsync(materialId, "Material");

            // Map existing attachments by Id for quick lookup
            var existingDict = existingAttachments.ToDictionary(a => a.Id);

            foreach (var attDto in dto.Attachments)
            {
                if (attDto.Id.HasValue && existingDict.TryGetValue(attDto.Id.Value, out var existingAtt))
                {
                    // Update existing attachment
                    existingAtt.FileType = attDto.FileType;
                    existingAtt.FileUrl = attDto.FileUrl;
                    existingAtt.FilePath = attDto.FilePath;
                    await _attachmentRepo.UpdateAsync(existingAtt);

                    // Remove from dictionary to track processed
                    existingDict.Remove(attDto.Id.Value);
                }
                else
                {
                    // New attachment
                    var newAtt = new Attachment
                    {
                        ReferenceId = materialId,
                        ReferenceType = "Material",
                        FileType = attDto.FileType,
                        FileUrl = attDto.FileUrl,
                        FilePath = attDto.FilePath,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _attachmentRepo.AddAsync(newAtt);
                }
            }

            // delete attachments not present in update DTO (if required)
            //if (existingDict.Any())
            //{
            //    await _attachmentRepo.DeleteRangeAsync(existingDict.Values);
            //}

            await transaction.CommitAsync();
            
            
            return new MaterialResponseDto
            {
                MaterialId = material.Id,
                Title = material.Title,
                ClassWorkId = material.ClassWorkId,
                Attachments = (await _attachmentRepo.GetByReferenceAsync(material.Id, "Material"))
                    .Select(a => new AttachmentResponseDto
                    {
                        Id = a.Id,
                        FileType = a.FileType,
                        FileUrl = a.FileUrl
                    }).ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteMaterialAsync(int materialId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Delete attachments linked to material
            var attachments = await _attachmentRepo.GetByReferenceAsync(materialId, "Material");
            if (attachments.Any())
            {
                await _attachmentRepo.DeleteRangeAsync(attachments);
            }

            // Delete material
            var deleted = await _materialRepo.DeleteAsync(materialId);
            if (!deleted) return false;

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}