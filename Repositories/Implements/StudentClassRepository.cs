using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class StudentClassRepository : IStudentClassRepository
{
    public async Task<List<int>> GetStudentIdsByClassIdAsync(int classId)
    {
        throw new NotImplementedException();
    }
}