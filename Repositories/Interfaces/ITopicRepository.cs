using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllTopicsAsync(int classId);
    Task<Topic?> GetTopicByIdAsync(int topicId);
    Task<Topic> AddTopicAsync(Topic topic);
    Task<Topic?> UpdateTopicAsync(Topic topic);
    Task<bool> DeleteTopicAsync(int topicId);
}