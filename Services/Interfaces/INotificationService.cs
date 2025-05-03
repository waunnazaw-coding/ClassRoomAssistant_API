using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface INotificationService
{
    Task<List<UserNotificationDto>> GetUserNotificationsAsync(int userId);
    Task<bool> MarkNotificationAsReadAsync(int notificationId);
    Task<int> MarkMultiNotificationsAsReadAsync(List<int> notificationIds);
}