using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class ToDoRepository : IToDoRepository
{
    private readonly DbContextClassName _context;
    public ToDoRepository(DbContextClassName context) => _context = context;

    public async Task BulkAddTodosAsync(IEnumerable<Todo> todos)
    {
        // Add range for batch insert
        await _context.Todos.AddRangeAsync(todos);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateDueDateByClassWorkIdAsync(int classWorkId, DateTime newDueDate)
    {
        var todos = await _context.Todos.Where(t => t.ClassWorkId == classWorkId).ToListAsync();
        foreach (var todo in todos)
        {
            todo.DueDate = newDueDate;
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByClassWorkIdAsync(int classWorkId)
    {
        var todos = await _context.Todos.Where(t => t.ClassWorkId == classWorkId).ToListAsync();
        _context.Todos.RemoveRange(todos);
        await _context.SaveChangesAsync();
    }
    
    

}

