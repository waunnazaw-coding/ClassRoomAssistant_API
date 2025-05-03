using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IAnnouncementService
{
    Task<IEnumerable<AnnouncementWithMessagesDto>> GetAnnouncementsWithMessagesAsync();
    Task<IEnumerable<AnnouncementResponseDto>> GetAnnouncementsByClassIdAsync(int classId);
    Task<AnnouncementResponseDto> CreateAnnouncementAsync(AnnouncementCreateRequestDto dto);
    Task UpdateAnnouncementAsync(int id, AnnouncementCreateRequestDto dto);
    Task DeleteAnnouncementAsync(int id);
}