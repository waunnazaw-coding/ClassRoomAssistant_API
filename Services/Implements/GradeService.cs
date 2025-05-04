using ClassRoomClone_App.Server.DTOs.GreadeDtos;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<GradeResponseDto> CreateGradeAsync(GradeCreateRequestDto dto)
    {
        // Get MaxScore from Assignments
        var maxScore = await _gradeRepository.GetMaxScoreForClassWorkAsync(dto.ClassWorkId);
        if (maxScore == null)
            throw new Exception("Assignment for the given ClassWorkId not found.");

        var grade = new Grade
        {
            StudentId = dto.StudentId,
            ClassWorkId = dto.ClassWorkId,
            Score = dto.Score,
            MaxScore = maxScore.Value,
            GradedBy = dto.GradedBy,
            CreatedAt = DateTime.UtcNow
        };

        var createdGrade = await _gradeRepository.AddGradeAsync(grade);

        return new GradeResponseDto
        {
            Id = createdGrade.Id,
            StudentId = createdGrade.StudentId,
            ClassWorkId = createdGrade.ClassWorkId,
            Score = createdGrade.Score,
            MaxScore = createdGrade.MaxScore,
            GradedBy = createdGrade.GradedBy,
            CreatedAt = createdGrade.CreatedAt
        };
    }
    
    public async Task<GradeResponseDto> UpdateGradeAsync(int gradeId, GradeUpdateRequestDto dto)
    {
        var grade = await _gradeRepository.GetGradeByIdAsync(gradeId);
        if (grade == null)
            throw new Exception("Grade not found.");

        // Optionally re-fetch MaxScore if you want to ensure it is current
        var maxScore = await _gradeRepository.GetMaxScoreForClassWorkAsync(grade.ClassWorkId);
        if (maxScore == null)
            throw new Exception("Assignment for the grade's ClassWorkId not found.");

        grade.Score = dto.Score;
        grade.GradedBy = dto.GradedBy;
        grade.MaxScore = maxScore.Value;

        var updatedGrade = await _gradeRepository.UpdateGradeAsync(grade);

        return new GradeResponseDto
        {
            Id = updatedGrade.Id,
            StudentId = updatedGrade.StudentId,
            ClassWorkId = updatedGrade.ClassWorkId,
            Score = updatedGrade.Score,
            MaxScore = updatedGrade.MaxScore,
            GradedBy = updatedGrade.GradedBy,
            CreatedAt = updatedGrade.CreatedAt
        };
    }
}