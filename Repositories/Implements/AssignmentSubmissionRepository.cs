using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class AssignmentSubmissionRepository : IAssignmentSubmissionRepository
{
    private readonly DbContextClassName _context;

    public AssignmentSubmissionRepository(DbContextClassName context)
    {
        _context = context;
    }
    
    public async Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
    {
        return await _context.AssignmentSubmissions
            .AsNoTracking()
            .Include(s => s.SubmissionResponses)
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync();
    }

    public async Task<AssignmentSubmission> AddAsync(AssignmentSubmission submission)
    {
        await _context.AssignmentSubmissions.AddAsync(submission);
        await _context.SaveChangesAsync();
        return submission;
    }
}