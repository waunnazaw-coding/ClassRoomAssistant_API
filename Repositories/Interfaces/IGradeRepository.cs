using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetGradeByIdAsync(int gradeId);
    Task<decimal?> GetMaxScoreForClassWorkAsync(int classWorkId);
    Task<Grade> AddGradeAsync(Grade grade);
    Task<Grade> UpdateGradeAsync(Grade grade);
}