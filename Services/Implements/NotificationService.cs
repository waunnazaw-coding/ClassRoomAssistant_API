using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepo;
    public NotificationService(INotificationRepository notificationRepository) => _notificationRepo = notificationRepository;

    public async Task<List<UserNotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var rawList = await _notificationRepo.GetUserNotificationsRawAsync(userId);

        return rawList.Select(n => new UserNotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            ReferenceId = n.ReferenceId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            Details = n.Type switch
            {
                "Announcement" => n.AnnouncementTitle ?? "N/A",
                "Message" => n.MessageContent ?? "N/A",
                "Assignment" => n.AssignmentTitle ?? "N/A",
                "Material" => n.MaterialTitle ?? "N/A",
                _ => "N/A"
            }
        }).ToList();
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
    {
        return await _notificationRepo.MarkNotificationAsReadAsync(notificationId);
    }

    public async Task<int> MarkMultiNotificationsAsReadAsync(List<int> notificationIds)
    {
        return await _notificationRepo.MarkMultiNotificationsAsReadAsync(notificationIds);
    }
}