namespace ClassRoomClone_App.Server.DTOs.GreadeDtos;

public class GradeUpdateRequestDto
{
    public decimal Score { get; set; }
    public int GradedBy { get; set; }
}