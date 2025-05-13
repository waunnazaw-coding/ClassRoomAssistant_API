namespace ClassRoomClone_App.Server.DTOs;

public class GetClassDetailsResponse
{
    public List<ClassDetailDto> Details { get; set; } = new List<ClassDetailDto>();
}