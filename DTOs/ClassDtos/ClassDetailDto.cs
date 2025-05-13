namespace ClassRoomClone_App.Server.DTOs;

public class ClassDetailDto
{
    public int EntityId { get; set; }
    public string EntityType { get; set; }       
    public string Content { get; set; }          
    public DateTime ActivityDate { get; set; }
    public int? MessageId { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string MessageContent { get; set; }   
    public DateTime? MessageCreatedAt { get; set; }
}