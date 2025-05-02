using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/classes/{classId:int}/topics")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        // GET: api/classes/{classId}/topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetAllTopics(int classId)
        {
            var topics = await _topicService.GetAllTopicsAsync(classId);
            return Ok(topics);
        }

        // GET: api/classes/{classId}/topics/{topicId}
        [HttpGet("{topicId:int}")]
        public async Task<ActionResult<TopicDto>> GetTopicById(int topicId)
        {
            try
            {
                var topic = await _topicService.GetTopicByIdAsync(topicId);
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // POST: api/classes/{classId}/topics
        [HttpPost]
        public async Task<ActionResult<TopicDto>> CreateTopic(int classId, [FromBody] CreateTopicDto createDto)
        {
            if (createDto == null)
                return BadRequest("Topic data is required.");

            if (classId != createDto.ClassId)
                return BadRequest("ClassId mismatch.");

            var topic = await _topicService.CreateTopicAsync(createDto);

            return CreatedAtAction(nameof(GetTopicById), new { classId, topicId = topic.Id }, topic);
        }

        // PUT: api/classes/{classId}/topics/{topicId}
        [HttpPut("{topicId:int}")]
        public async Task<ActionResult<TopicDto>> UpdateTopic(int topicId, [FromBody] UpdateTopicDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Topic data is required.");

            try
            {
                var updatedTopic = await _topicService.UpdateTopicAsync(topicId, updateDto);
                return Ok(updatedTopic);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/classes/{classId}/topics/{topicId}
        [HttpDelete("{topicId:int}")]
        public async Task<ActionResult> DeleteTopic(int topicId)
        {
            var deleted = await _topicService.DeleteTopicAsync(topicId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
