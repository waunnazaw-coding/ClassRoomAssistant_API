namespace ClassRoomClone_App.Server.DTOs;

// Detailed response DTO including participants
public class ClassDetailsResponseDto : ClassResponseDto
{
    public List<ClassParticipantResponseDto> Participants { get; set; } = new List<ClassParticipantResponseDto>();
}