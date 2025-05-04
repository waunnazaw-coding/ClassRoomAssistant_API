using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class GradeRepository : IGradeRepository
{
    private readonly DbContextClassName _context;

    public GradeRepository(DbContextClassName context)
    {
        _context = context;
    }

    // Get MaxScore (Points) from Assignments table for the given ClassWorkId
    public async Task<decimal?> GetMaxScoreForClassWorkAsync(int classWorkId)
    {
        return await _context.Assignments
            .AsNoTracking()
            .Where(a => a.ClassWorkId == classWorkId)
            .Select(a => (decimal?)a.Points)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Grade?> GetGradeByIdAsync(int gradeId)
    {
        return await _context.Grades.FindAsync(gradeId);
    }

    // Add Grade record
    public async Task<Grade> AddGradeAsync(Grade grade)
    {
        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();
        return grade;
    }
    
    public async Task<Grade> UpdateGradeAsync(Grade grade)
    {
        _context.Grades.Update(grade);
        await _context.SaveChangesAsync();
        return grade;
    }
}