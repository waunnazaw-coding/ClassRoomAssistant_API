using ClassRoomClone_App.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task<IEnumerable<UserClassesRawDto>> GetClassesByUserId(int userId);
        Task<IEnumerable<ClassDetailsWithEntityId>> GetClassDetailsWithEntityIdAsync(int classId);
        Task<IEnumerable<Class>> GetArchivedClassesAsync();
        Task<Class?> GetClassByIdAsync(int id);
        Task<List<ClassDetailDto>> GetClassDetailsAsync(int id);
        Task<bool> ApproveParticipantAsync(int userId, int classId);
        Task<Class> AddClassAsync(Class entity);
        Task<Class?> UpdateClassAsync(Class entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<bool> ActualDeleteAsync(int id);
        Task<bool> ClassCodeExistsAsync(string classCode);
        Task<Class?> GetClassByCodeAsync(string classCode);
        Task<bool> StudentExistsInClassAsync(int classId, int studentId);
        Task AddClassParticipantAsync(ClassParticipant participant);
    }
}