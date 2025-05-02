using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class ClassWorkRepository : IClassWorkRepository
{
    private readonly DbContextClassName _context;
    public ClassWorkRepository(DbContextClassName context) => _context = context;

    
    public async Task<List<Topic>> GetAllTopicsWithClassWorksAsync(int classId)
    {
        // Eager load ClassWorks, Materials, Assignments filtered by classId
        return await _context.Topics
            .Include(t => t.ClassWorks.Where(cw => cw.ClassId == classId))
            .ThenInclude(cw => cw.Materials)
            .Include(t => t.ClassWorks.Where(cw => cw.ClassId == classId))
            .ThenInclude(cw => cw.Assignments)
            //.OrderBy(t => t.Name)
            .ToListAsync();
    }
    
    public async Task<ClassWork> AddClassWorkAsync(ClassWork classWork)
    {
        await _context.ClassWorks.AddAsync(classWork);
        await _context.SaveChangesAsync();
        return classWork;
    }
    
    public async Task DeleteAsync(int classWorkId)
    {
        var classWork = await _context.ClassWorks.FindAsync(classWorkId);
        if (classWork != null)
        {
            _context.ClassWorks.Remove(classWork);
            await _context.SaveChangesAsync();
        }
    }

}
