using ClassRoomClone_App.Server.DTOs.GreadeDtos;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IGradeService
{
    Task<GradeResponseDto> CreateGradeAsync(GradeCreateRequestDto dto);
    
    Task<GradeResponseDto> UpdateGradeAsync(int gradeId, GradeUpdateRequestDto dto);
}