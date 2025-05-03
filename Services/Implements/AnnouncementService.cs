using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IClassParticipantsRepository _classParticipantRepo;
    private readonly DbContextClassName _context;

    public AnnouncementService(
        IAnnouncementRepository announcementRepo,
        INotificationRepository notificationRepo,
        IClassParticipantsRepository classParticipantRepo,
        DbContextClassName context)
    {
        _announcementRepo = announcementRepo;
        _notificationRepo = notificationRepo;
        _classParticipantRepo = classParticipantRepo;
        _context = context;
    }
    public async Task<IEnumerable<AnnouncementWithMessagesDto>> GetAnnouncementsWithMessagesAsync()
    {
        return await _announcementRepo.GetAnnouncementsWithMessagesAsync();
    }
    
    public async Task<IEnumerable<AnnouncementResponseDto>> GetAnnouncementsByClassIdAsync(int classId)
    {
        var announcements = await _announcementRepo.GetByClassIdAsync(classId);

        return announcements.Select(a => new AnnouncementResponseDto
        {
            Id = a.Id,
            ClassId = a.ClassId,
            Title = a.Title,
            Message = a.Message,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
    
    public async Task<AnnouncementResponseDto> CreateAnnouncementAsync(AnnouncementCreateRequestDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var announcement = new Announcement
        {
            ClassId = dto.ClassId,
            Title = dto.Title,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        await _announcementRepo.AddAsync(announcement);

        var userIds = await _classParticipantRepo.GetUserIdsByClassIdAsync(dto.ClassId);

        var notifications = userIds.Select(userId => new Notification
        {
            UserId = userId,
            Type = "Announcement",
            ReferenceId = announcement.Id,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        await _notificationRepo.AddRangeAsync(notifications);

        await transaction.CommitAsync();

        return new AnnouncementResponseDto
        {
            Id = announcement.Id,
            ClassId = announcement.ClassId,
            Title = announcement.Title,
            Message = announcement.Message,
            CreatedAt = announcement.CreatedAt
        };
    }

    public async Task UpdateAnnouncementAsync(int id, AnnouncementCreateRequestDto dto)
    {
        var announcement = await _announcementRepo.GetByIdAsync(id);
        if (announcement == null)
            throw new KeyNotFoundException("Announcement not found");

        announcement.Title = dto.Title;
        announcement.Message = dto.Message;

        await _announcementRepo.UpdateAsync(announcement);
    }

    public async Task DeleteAnnouncementAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        await _notificationRepo.DeleteByReferenceAsync("Announcement", id);
        await _announcementRepo.DeleteAsync(id);

        await transaction.CommitAsync();
    }
    
    
}