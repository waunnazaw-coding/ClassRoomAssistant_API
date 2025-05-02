namespace ClassRoomClone_App.Server.DTOs;

public class TopicWithMaterialsAssignmentsDto
{
    public int TopicId { get; set; }
    public string TopicName { get; set; } = null!;
    public List<MaterialDto> Materials { get; set; } = new();
    public List<AssignmentDto> Assignments { get; set; } = new();
}

