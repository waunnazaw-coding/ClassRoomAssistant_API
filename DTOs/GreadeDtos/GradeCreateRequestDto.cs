namespace ClassRoomClone_App.Server.DTOs.GreadeDtos;

public class GradeCreateRequestDto
{
    public int StudentId { get; set; }
    public int ClassWorkId { get; set; }
    public decimal Score { get; set; }
    public int GradedBy { get; set; }
}