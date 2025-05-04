namespace ClassRoomClone_App.Server.DTOs.GreadeDtos;

public class GradeResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ClassWorkId { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public int GradedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}