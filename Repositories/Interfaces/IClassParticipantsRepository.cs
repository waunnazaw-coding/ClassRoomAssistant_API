using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Repositories.Interfaces
{
    public interface IClassParticipantsRepository
    {
        Task<IEnumerable<ClassParticipant>> GetAllParticipantsAsync(int classId);
        
        Task<IEnumerable<int>> GetUserIdsByClassIdAsync(int classId);

        Task<ClassParticipant> SetMainTeacherAsync(int userId, int classId);

        Task<ClassParticipant> AddSubTeacherAsync(int userId, int classId);

        Task<bool> TransferOwnershipAsync(int classId, int currentOwnerId, int newOwnerId);

        Task<bool> RemoveSubTeacherAsync(int userId, int classId);

        Task<bool> RemoveStudentAsync(int userId, int classId);

        Task<ClassParticipant> AddStudentAsync(int userId, int classId);
        
        Task<List<int>> GetStudentUserIdsByClassIdAsync(int classId);
    }
}