using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    // GET: api/topics
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetAllTopics(int userId)
    {
        var topics = await _topicService.GetAllTopicsAsync( userId);
        return Ok(topics);
    }

    // GET: api/topics/{topicId}
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

    // POST: api/topics
    [HttpPost]
    public async Task<ActionResult<TopicDto>> CreateTopic([FromBody] CreateTopicDto createDto)
    {
        if (createDto == null)
            return BadRequest("Topic data is required.");

        var topic = await _topicService.CreateTopicAsync(createDto);

        return CreatedAtAction(nameof(GetTopicById), new { topicId = topic.Id }, topic);
    }

    // PUT: api/topics/{topicId}
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

    // DELETE: api/topics/{topicId}
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
