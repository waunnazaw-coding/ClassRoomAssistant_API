using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/classes/{classId:int}/announcements")]
    //[Authorize(Policy = "TeacherOrSubTeacher")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        
        [HttpGet("with-comments")]
        public async Task<ActionResult<List<AnnouncementWithMessagesDto>>> GetAnnouncementsWithMessages()
        {
            var announcements = await _announcementService.GetAnnouncementsWithMessagesAsync();
            return Ok(announcements);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementResponseDto>>> GetAnnouncementsByClassId(int classId)
        {
            var announcements = await _announcementService.GetAnnouncementsByClassIdAsync(classId);
            return Ok(announcements);
        }

        // POST: api/classes/{classId}/announcements
        [HttpPost]
        
        public async Task<ActionResult<AnnouncementResponseDto>> CreateAnnouncement(
            [FromRoute] int classId,
            [FromBody] AnnouncementCreateRequestDto dto)
        {
            if (dto.ClassId != classId)
                return BadRequest("Class ID mismatch.");

            try
            {
                var createdAnnouncement = await _announcementService.CreateAnnouncementAsync(dto);
                return createdAnnouncement;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PUT: api/classes/{classId}/announcements/{announcementId}
        [HttpPut("{announcementId:int}")]
        public async Task<IActionResult> UpdateAnnouncement(
            [FromRoute] int classId,
            [FromRoute] int announcementId,
            [FromBody] AnnouncementCreateRequestDto dto)
        {
            if (dto.ClassId != classId)
                return BadRequest("Class ID mismatch.");

            try
            {
                await _announcementService.UpdateAnnouncementAsync(announcementId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Announcement not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // DELETE: api/classes/{classId}/announcements/{announcementId}
        [HttpDelete("{announcementId:int}")]
        public async Task<IActionResult> DeleteAnnouncement(
            [FromRoute] int classId,
            [FromRoute] int announcementId)
        {
            try
            {
                await _announcementService.DeleteAnnouncementAsync(announcementId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Announcement not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
