using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync(int classId)
        {
            var topics = await _topicRepository.GetAllTopicsAsync(classId);
            return topics.Select(MapToDto);
        }

        public async Task<TopicDto> GetTopicByIdAsync(int topicId)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(topicId);
            if (topic == null)
                throw new Exception("Topic not found");

            return MapToDto(topic);
        }

        public async Task<TopicDto> CreateTopicAsync(CreateTopicDto createDto)
        {
            var topic = new Topic
            {
                ClassId = createDto.UserId,
                Title = createDto.Title,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _topicRepository.AddTopicAsync(topic);
            return MapToDto(created);
        }

        public async Task<TopicDto> UpdateTopicAsync(int topicId, UpdateTopicDto updateDto)
        {
            var topic = new Topic
            {
                ClassId = updateDto.UserId,
                Id = topicId,
                Title = updateDto.Title
            };

            var updated = await _topicRepository.UpdateTopicAsync(topic);
            if (updated == null)
                throw new Exception("Topic not found");

            return MapToDto(updated);
        }

        public async Task<bool> DeleteTopicAsync(int topicId)
        {
            return await _topicRepository.DeleteTopicAsync(topicId);
        }

        private TopicDto MapToDto(Topic topic)
        {
            return new TopicDto
            {
                Id = topic.Id,
                UserId = topic.ClassId,
                Title = topic.Title,
                CreatedAt = topic.CreatedAt
            };
        }
    }

}
