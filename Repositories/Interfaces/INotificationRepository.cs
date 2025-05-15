using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<UserNotificationRawDto>> GetUserNotificationsRawAsync(int userId);
    Task AddRangeAsync(IEnumerable<Notification> notifications);
    Task<bool> MarkNotificationAsReadAsync(int notificationId);
    Task<int> MarkMultiNotificationsAsReadAsync(List<int> notificationIds);
    Task DeleteByReferenceAsync(string type, int referenceId);
    Task SaveChangesAsync();
}