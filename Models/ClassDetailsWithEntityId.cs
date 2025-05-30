namespace ClassRoomClone_App.Server.Models;

public class ClassDetailsWithEntityId
{
    public int EntityId { get; set; }
    public string EntityType { get; set; }
    public string Content { get; set; }
    public DateTime ActivityDate { get; set; }
    public int? AnnouncementId { get; set; }
    public int? AssignmentId { get; set; }
    public int? MaterialId { get; set; }
    public string MessageContent { get; set; }
    public DateTime? MessageCreatedAt { get; set; }
        
}