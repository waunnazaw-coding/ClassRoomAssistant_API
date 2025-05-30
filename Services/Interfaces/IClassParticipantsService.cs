using ClassRoomClone_App.Server.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Services.Interfaces
{
    public interface IClassParticipantsService
    {
        Task<IEnumerable<ClassParticipantResponseDto>> GetAllParticipantsAsync(int classId);

        Task<ClassParticipantResponseDto> SetMainTeacherAsync(int userId, int classId);

        Task<string?> GetRetrieveRoleAsyn(int userId, int classId);

        Task<ClassParticipantResponseDto> AddSubTeacherAsync(int userId, int classId);
        
        Task AddSubTeacherToClassAsync(int teacherUserId, int classId, string email);

        Task AddStudentToClassAsync(int teacherUserId, int classId, string email);

        Task<bool> TransferOwnershipAsync(int classId, int currentOwnerId, int newOwnerId);

        Task<bool> RemoveSubTeacherAsync(int userId, int classId);

        Task<bool> RemoveStudentAsync(int userId, int classId);

        Task<ClassParticipantResponseDto> AddStudentAsync(int userId, int classId);
    }
}