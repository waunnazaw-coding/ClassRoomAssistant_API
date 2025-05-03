using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public CommentController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        // POST: api/comments
        [HttpPost]
        public async Task<ActionResult<MessageResponseDto>> CreateMessage([FromBody] MessageCreateRequestDto dto)
        {
            try
            {
                var createdMessage = await _messageService.CreateMessageAsync(dto);
                return createdMessage;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PUT: api/comments/{commentId}
        [HttpPut("{messageId:int}")]
        public async Task<IActionResult> UpdateMessage(int messageId, [FromBody] MessageCreateRequestDto dto)
        {
            try
            {
                await _messageService.UpdateMessageAsync(messageId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Message not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // DELETE: api/comments/{commentId}
        [HttpDelete("{messageId:int}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Message not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
