using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.Helpers;

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
        public async Task<ActionResult<ApiResponse<IEnumerable<AnnouncementWithMessagesDto>>>> GetAnnouncementsWithMessages()
        {
            var announcements = await _announcementService.GetAnnouncementsWithMessagesAsync();
            return Ok(new ApiResponse<IEnumerable<AnnouncementWithMessagesDto>>(announcements, true, "Announcements with messages retrieved successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AnnouncementResponseDto>>>> GetAnnouncementsByClassId(int classId)
        {
            var announcements = await _announcementService.GetAnnouncementsByClassIdAsync(classId);
            return Ok(new ApiResponse<IEnumerable<AnnouncementResponseDto>>(announcements, true, "Announcements retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AnnouncementResponseDto>>> CreateAnnouncement(
            [FromRoute] int classId,
            [FromBody] AnnouncementCreateRequestDto dto)
        {
            if (dto.ClassId != classId)
                return BadRequest(new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, false, "Class ID mismatch."));

            try
            {
                var createdAnnouncement = await _announcementService.CreateAnnouncementAsync(dto);
                return CreatedAtAction(nameof(GetAnnouncementsByClassId), new { classId = classId }, 
                    new ApiResponse<AnnouncementResponseDto>(createdAnnouncement, true, "Announcement created successfully."));
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                return StatusCode(500, new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, false, "An unexpected error occurred."));
            }
        }

        [HttpPut("{announcementId:int}")]
        public async Task<ActionResult<ApiResponse<AnnouncementResponseDto>>> UpdateAnnouncement(
            [FromRoute] int classId,
            [FromRoute] int announcementId,
            [FromBody] AnnouncementCreateRequestDto dto)
        {
            if (dto.ClassId != classId)
                return BadRequest(new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, false, "Class ID mismatch."));

            try
            {
                await _announcementService.UpdateAnnouncementAsync(announcementId, dto);
                return Ok(new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, true, "Announcement updated successfully."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, false, "Announcement not found."));
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                return StatusCode(500, new ApiResponse<AnnouncementResponseDto>((AnnouncementResponseDto)null, false, "An unexpected error occurred."));
            }
        }

        [HttpDelete("{announcementId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAnnouncement(
            [FromRoute] int classId,
            [FromRoute] int announcementId)
        {
            try
            {
                await _announcementService.DeleteAnnouncementAsync(announcementId);
                return Ok(new ApiResponse<object>(null, true, "Announcement deleted successfully."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>(null, false, "Announcement not found."));
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                return StatusCode(500, new ApiResponse<object>(null, false, "An unexpected error occurred."));
            }
        }
    }
    
}
