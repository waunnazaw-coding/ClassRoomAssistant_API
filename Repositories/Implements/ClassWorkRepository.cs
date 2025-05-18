using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class ClassWorkRepository : IClassWorkRepository
{
    private readonly DbContextClassName _context;
    public ClassWorkRepository(DbContextClassName context) => _context = context;

    
    //Explicit loading
    public async Task<List<Topic>> GetAllTopicsWithClassWorksAsync(int classId)
    {
        // Load topics filtered by classId
        var topics = await _context.Topics
            .AsTracking()
            .Where(t => t.ClassId == classId)
            .ToListAsync();
    
        foreach (var topic in topics)
        {
            // Explicitly load ClassWorks for each topic, filtering by classId
            await _context.Entry(topic)
                .Collection(t => t.ClassWorks)
                .Query()
                .Where(cw => cw.ClassId == classId)
                .LoadAsync();
    
            foreach (var classWork in topic.ClassWorks)
            {
                // Explicitly load Materials for each ClassWork
                await _context.Entry(classWork)
                    .Collection(cw => cw.Materials)
                    .LoadAsync();
    
                // Explicitly load Assignments for each ClassWork
                await _context.Entry(classWork)
                    .Collection(cw => cw.Assignments)
                    .LoadAsync();
            }
        }
    
        return topics;
    }
    
    public async Task<Topic?> FilterByTopicAsync(int topicId, int classId)
    {
        // Load the topic filtered by topicId and classId
        var topic = await _context.Topics
            .Where(t => t.Id == topicId && t.ClassId == classId)
            .FirstOrDefaultAsync();
    
        if (topic == null)
            return null;
    
        //  Explicitly load ClassWorks filtered by classId
        await _context.Entry(topic)
            .Collection(t => t.ClassWorks)
            .Query()
            .Where(cw => cw.ClassId == classId)
            .LoadAsync();
    
        //  For each ClassWork, explicitly load Materials and Assignments
        foreach (var classWork in topic.ClassWorks)
        {
            await _context.Entry(classWork)
                .Collection(cw => cw.Materials)
                .LoadAsync();
    
            await _context.Entry(classWork)
                .Collection(cw => cw.Assignments)
                .LoadAsync();
        }
    
        return topic;
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
