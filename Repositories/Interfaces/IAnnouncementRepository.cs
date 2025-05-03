using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IAnnouncementRepository
{
    Task<List<AnnouncementWithMessagesDto>> GetAnnouncementsWithMessagesAsync();
    Task<IEnumerable<Announcement>> GetByClassIdAsync(int classId);
    Task<Announcement> AddAsync(Announcement announcement);
    Task<Announcement?> GetByIdAsync(int id);
    Task UpdateAsync(Announcement announcement);
    Task DeleteAsync(int id);
}