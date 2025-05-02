using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IMaterialRepository
{
    Task<Material?> GetByIdWithAttachmentsAsync(int materialId);
    Task<Material?> GetByIdAsync(int materialId);
    Task<Material> UpdateAsync(Material material);
    Task<bool> DeleteAsync(int materialId);
    Task<Material> AddAsync(Material material);
}