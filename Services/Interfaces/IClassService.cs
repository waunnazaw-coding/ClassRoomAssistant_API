using ClassRoomClone_App.Server.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync();
        Task<IEnumerable<ClassResponseDto>> GetArchivedClassesAsync();
        Task<IEnumerable<UserClassesRawDto>> GetClassesByUserId(int userId);
        Task<ClassResponseDto> GetClassByIdAsync(int id);
        Task<ClassDetailsResponseDto> GetClassDetailsAsync(int id);
        Task<ClassResponseDto> AddClassAsync(ClassRequestDto requestDto, int userId);
        Task<ClassResponseDto?> GetClassByCodeAsync(string classCode);
        Task<bool> EnrollStudentInClassAsync(string classCode, int studentId);
        Task<ClassResponseDto> UpdateClassAsync(int id, ClassUpdateRequestDto requestDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<bool> ActualDeleteAsync(int id);
    }
}