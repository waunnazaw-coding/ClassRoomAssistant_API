using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicDto>> GetAllTopicsAsync();
        Task<TopicDto> GetTopicByIdAsync(int topicId);
        Task<TopicDto> CreateTopicAsync(CreateTopicDto createDto);
        Task<TopicDto> UpdateTopicAsync(int topicId, UpdateTopicDto updateDto);
        Task<bool> DeleteTopicAsync(int topicId);
    }
}