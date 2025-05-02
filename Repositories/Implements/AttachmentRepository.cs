using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly DbContextClassName _context;

    public AttachmentRepository(DbContextClassName context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Attachment> attachments)
    {
        await _context.Attachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Attachment>> GetByReferenceAsync(int referenceId, string referenceType)
    {
        return await _context.Attachments
            .Where(a => a.ReferenceId == referenceId && a.ReferenceType == referenceType)
            .ToListAsync();
    }

    public async Task UpdateAsync(Attachment attachment)
    {
        _context.Attachments.Update(attachment);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Attachment attachment)
    {
        await _context.Attachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<Attachment> attachments)
    {
        _context.Attachments.RemoveRange(attachments);
        await _context.SaveChangesAsync();
    }
}