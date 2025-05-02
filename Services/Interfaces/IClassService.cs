using ClassRoomClone_App.Server.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync();
        Task<IEnumerable<ClassResponseDto>> GetArchivedClassesAsync();
        Task<ClassResponseDto> GetClassByIdAsync(int id);
        Task<ClassDetailsResponseDto> GetClassDetailsAsync(int id);
        Task<ClassResponseDto> AddClassAsync(ClassRequestDto requestDto, int userId);
        Task<ClassResponseDto> UpdateClassAsync(int id, ClassRequestDto requestDto);
        Task<bool> DeleteAsync(int id);
    }
}