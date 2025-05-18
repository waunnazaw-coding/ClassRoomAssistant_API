using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IClassWorkService
{
    Task<List<TopicWithMaterialsAssignmentsDto>> GetTopicsWithMaterialsAndAssignmentsAsync(int classId);
    Task<TopicWithMaterialsAssignmentsDto?> FilterByTopicAsync(int topicId, int classId);
}