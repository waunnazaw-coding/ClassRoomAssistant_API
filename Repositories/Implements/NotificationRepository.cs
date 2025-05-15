using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class NotificationRepository : INotificationRepository
{
    private readonly DbContextClassName _context;

    public NotificationRepository(DbContextClassName context)
    {
        _context = context;
    }
    
    public async Task<List<UserNotificationRawDto>> GetUserNotificationsRawAsync(int userId)
    {
        return await _context.UserNotificationRawDtos
            .FromSqlInterpolated($@"EXEC GetUserNotifications @TargetUserId = {userId}")
            .ToListAsync();
    }
    
    public async Task AddRangeAsync(IEnumerable<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId); // [2][5][9]
        if (notification == null)
            return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<int> MarkMultiNotificationsAsReadAsync(List<int> notificationIds)
    {
        var notifications = await _context.Notifications
            .Where(n => notificationIds.Contains(n.Id))
            .ToListAsync();

        foreach (var n in notifications)
            n.IsRead = true;

        return await _context.SaveChangesAsync();
    }



    public async Task DeleteByReferenceAsync(string type, int referenceId)
    {
        var notifications = _context.Notifications.Where(n => n.Type == type && n.ReferenceId == referenceId);
        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}