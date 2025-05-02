using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IToDoRepository
{
    Task BulkAddTodosAsync(IEnumerable<Todo> todos);
    
    Task UpdateDueDateByClassWorkIdAsync(int classWorkId, DateTime newDueDate);
    
    Task DeleteByClassWorkIdAsync(int classWorkId);
    
    //Task<IEnumerable<AssignmentWithStatusDto>> GetAssignmentsWithStatusAsync(int userId);

}