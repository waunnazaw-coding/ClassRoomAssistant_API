
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly DbContextClassName _context;

    public AssignmentRepository(DbContextClassName context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Assignment>> GetAllAsync(int classWorkId)
    {
        return await _context.Assignments
            .AsNoTracking()
            .Where(a => a.ClassWorkId == classWorkId)
            .ToListAsync();
    }

    public async Task<Assignment?> GetByIdAsync(int id)
    {
        return await _context.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment> AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();

        return assignment;
    }

    public async Task<Assignment?> UpdateAsync(Assignment assignment)
    {
        var existing = await _context.Assignments.FindAsync(assignment.Id);
        if(existing == null) return null;
        
        existing.Title = assignment.Title;
        existing.Instructions = assignment.Instructions;
        existing.Points = assignment.Points;
        existing.DueDate = assignment.DueDate;
        existing.AllowLateSubmission = assignment.AllowLateSubmission;
        
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Assignments.FindAsync(id);
        if(existing == null) return false;
        
        _context.Assignments.Remove(existing);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<IEnumerable<Assignment>> GetAssignmentsWithClassInfoAsync(int userId)
    {
        var userClassIds = await _context.ClassParticipants
            .Where(cp => cp.UserId == userId)
            .Select(cp => cp.ClassId)
            .ToListAsync();

        return await _context.Assignments
            .Include(a => a.ClassWork)
            .ThenInclude(cw => cw.Class)
            .Where(a => userClassIds.Contains(a.ClassWork.ClassId))
            .ToListAsync();
    }
}