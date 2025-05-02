namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IStudentClassRepository
{
    Task<List<int>> GetStudentIdsByClassIdAsync(int classId);
}