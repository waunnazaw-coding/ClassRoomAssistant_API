using ClassRoomClone_App.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task<IEnumerable<Class>> GetArchivedClassesAsync();
        Task<Class?> GetClassByIdAsync(int id);
        Task<Class?> GetClassDetailsAsync(int id);
        Task<Class> AddClassAsync(Class entity);
        Task<Class?> UpdateClassAsync(Class entity);
        Task<bool> DeleteAsync(int id);
    }
}