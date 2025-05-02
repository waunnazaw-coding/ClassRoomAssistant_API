
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IAssignmentRepository
{
    Task<IEnumerable<Assignment>> GetAllAsync(int classWorkId);
    Task<Assignment?> GetByIdAsync(int id);
    Task<Assignment> AddAsync(Assignment assignment);
    Task<Assignment?> UpdateAsync(Assignment assignment);
    Task<bool> DeleteAsync(int id);
    
    Task<IEnumerable<Assignment>> GetAssignmentsWithClassInfoAsync(int userId);
}