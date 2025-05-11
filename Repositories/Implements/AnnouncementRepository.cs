using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly DbContextClassName _context;

    public AnnouncementRepository(DbContextClassName context)
    {
        _context = context;
    }
    
    public async Task<List<AnnouncementWithMessagesDto>> GetAnnouncementsWithMessagesAsync()
    {
        // Query announcements ordered by CreatedAt descending
        var announcements = await _context.Announcements
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AnnouncementWithMessagesDto
            {
                AnnouncementId = a.Id,
                AnnouncementMessage = a.Message,
                AnnouncementCreatedAt = a.CreatedAt,
                Messages = _context.Messages
                    .Where(m => m.ParentType == "Announcement" && m.ParentId == a.Id)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new MessageDto
                    {
                        MessageId = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        CommentMessage = m.Message1,
                        IsPrivate = m.IsPrivate,
                        CommentCreatedAt = m.CreatedAt
                    })
                    .ToList()
            })
            .ToListAsync();

        return announcements;
    }
    
    public async Task<IEnumerable<Announcement>>  GetByClassIdAsync(int classId)
    {
        return await _context.Announcements
            .AsNoTracking()
            .Where(a => a.ClassId == classId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Announcement> AddAsync(Announcement announcement)
    {
        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }
    
    public async Task<Announcement?> GetByIdAsync(int id)
    {
        return await _context.Announcements.FindAsync(id);
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement != null)
        {
            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
        }
    }
}