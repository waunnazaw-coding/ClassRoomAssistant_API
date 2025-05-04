using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DbContextClassName _context;

        public TopicRepository(DbContextClassName context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            return await _context.Topics
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Topic?> GetTopicByIdAsync(int topicId)
        {
            return await _context.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == topicId);
        }

        public async Task<Topic> AddTopicAsync(Topic topic)
        {
            topic.CreatedAt = DateTime.UtcNow;

            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();

            return topic;
        }

        public async Task<Topic?> UpdateTopicAsync(Topic topic)
        {
            var existing = await _context.Topics.FindAsync(topic.Id);
            if (existing == null)
                return null;

            existing.Title = topic.Title;
            
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteTopicAsync(int topicId)
        {
            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
                return false;

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}