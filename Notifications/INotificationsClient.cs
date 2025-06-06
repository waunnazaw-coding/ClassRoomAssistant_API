namespace ClassRoomClone_App.Server.Notifications
{
    public interface INotificationsClient
    {
        Task ReceiveNotification(NotificationPayload notification);
    }

}


public class NotificationPayload
{
    public string Type { get; set; }  // e.g., "Assignment", "Message", "Alert"
    public string Title { get; set; }
    public string Message { get; set; }
    public string ClassName { get; set; }
    public int? EntityId { get; set; }
    public string DueDate { get; set; }
    // Add other common fields or a dictionary for extensibility
    public Dictionary<string, object> AdditionalData { get; set; }
}
