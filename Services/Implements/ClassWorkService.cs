using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements;

public class ClassWorkService : IClassWorkService
{
    private readonly IClassWorkRepository _classWorkRepo;

    public ClassWorkService(IClassWorkRepository classWorkRepo)
    {
        _classWorkRepo = classWorkRepo;
    }
    
    public async Task<List<TopicWithMaterialsAssignmentsDto>> GetTopicsWithMaterialsAndAssignmentsAsync(int classId)
    {
        var topics = await _classWorkRepo.GetAllTopicsWithClassWorksAsync(classId);

        return topics.Select(t => new TopicWithMaterialsAssignmentsDto
            {
                TopicId = t.Id,
                TopicName = t.Title,
                Materials = t.ClassWorks
                    .SelectMany(cw => cw.Materials)
                    .OrderByDescending(m => m.ClassWork.CreatedAt)
                    .Select(m => new MaterialDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        CreatedAt = m.ClassWork.CreatedAt
                    })
                    .ToList(),
                Assignments = t.ClassWorks
                    .SelectMany(cw => cw.Assignments)
                    .OrderByDescending(a => a.ClassWork.CreatedAt)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Points = a.Points,
                        CreatedAt = a.ClassWork.CreatedAt
                    })
                    .ToList()
            })
            .OrderBy(t => t.TopicName)
            .ToList();
    }
    
    public async Task<TopicWithMaterialsAssignmentsDto?> FilterByTopicAsync(int topicId, int classId)
    {
        var topic = await _classWorkRepo.FilterByTopicAsync(topicId, classId);

        if (topic == null)
            return null;
        
        var result = new TopicWithMaterialsAssignmentsDto
        {
            TopicId = topic.Id,
            TopicName = topic.Title,
            Materials = topic.ClassWorks
                .SelectMany(cw => cw.Materials)
                .OrderByDescending(m => m.ClassWork.CreatedAt)
                .Select(m => new MaterialDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    CreatedAt = m.ClassWork.CreatedAt
                })
                .ToList(),
            Assignments = topic.ClassWorks
                .SelectMany(cw => cw.Assignments)
                .OrderByDescending(a => a.ClassWork.CreatedAt)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Points = a.Points,
                    CreatedAt = a.ClassWork.CreatedAt
                })
                .ToList()  // Return the most recent assignment or null
        };

        return result;
    }


}
