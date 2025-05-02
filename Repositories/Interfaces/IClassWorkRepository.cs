using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IClassWorkRepository
{
    Task<List<Topic>> GetAllTopicsWithClassWorksAsync(int classId);
    
    Task<ClassWork> AddClassWorkAsync(ClassWork classWork);
    
    Task DeleteAsync(int classWorkId);

}