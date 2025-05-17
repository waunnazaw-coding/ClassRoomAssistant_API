using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class SubmissionResponseRepository : ISubmissionResponseRepository
{
    private readonly DbContextClassName _context;

    public SubmissionResponseRepository(DbContextClassName context)
    {
        _context = context;
    }
    
    public async Task<SubmissionResponse?> GetByIdWithSubmissionAsync(int responseId)
    {
        return await _context.SubmissionResponses
            .Include(r => r.Submission)
            .FirstOrDefaultAsync(r => r.Id == responseId);
    }

    public async Task UpdateAsync(SubmissionResponse response)
    {
        _context.SubmissionResponses.Update(response);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<SubmissionResponse> responses)
    {
        await _context.SubmissionResponses.AddRangeAsync(responses);
        await _context.SaveChangesAsync();
    }
    
    public async Task<SubmissionResponse> AddAsync(SubmissionResponse response)
    {
        await _context.SubmissionResponses.AddAsync(response);
        await _context.SaveChangesAsync();
        return response;
    }
}