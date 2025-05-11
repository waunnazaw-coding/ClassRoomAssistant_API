using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/classes/{classId:int}/participants")]
    public class ClassParticipantsController : ControllerBase
    {
        private readonly IClassParticipantsService _participantsService;

        public ClassParticipantsController(IClassParticipantsService participantsService)
        {
            _participantsService = participantsService;
        }

        // GET: api/classes/{classId}/participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassParticipantResponseDto>>> GetAllParticipants(int classId)
        {
            if (classId <= 0)
                return BadRequest("Invalid class ID.");

            var participants = await _participantsService.GetAllParticipantsAsync(classId);

            if (participants == null || !participants.Any())
                return NotFound("No participants found in this class.");

            return Ok(participants);
        }

        // POST: api/classes/{classId}/participants/sub-teachers/{userId}
        [HttpPost("sub-teachers/{userId:int}")]
        public async Task<ActionResult<ClassParticipantResponseDto>> AddSubTeacher(int classId, int userId)
        {
            if (classId <= 0 || userId <= 0)
                return BadRequest("Invalid class ID or user ID.");

            var participant = await _participantsService.AddSubTeacherAsync(userId, classId);

            return CreatedAtAction(nameof(GetAllParticipants), new { classId }, participant);
        }

        // DELETE: api/classes/{classId}/participants/sub-teachers/{userId}
        [HttpDelete("sub-teachers/{userId:int}")]
        public async Task<IActionResult> RemoveSubTeacher(int classId, int userId)
        {
            if (classId <= 0 || userId <= 0)
                return BadRequest("Invalid class ID or user ID.");

            var success = await _participantsService.RemoveSubTeacherAsync(userId, classId);

            if (!success)
                return BadRequest("Failed to remove sub-teacher.");

            return NoContent();
        }

        // PUT: api/classes/{classId}/participants/transfer-ownership?currentOwnerId=xxx&newOwnerId=yyy
        [HttpPut("transfer-ownership")]
        public async Task<IActionResult> TransferOwnership(int classId, [FromQuery] int currentOwnerId, [FromQuery] int newOwnerId)
        {
            if (classId <= 0 || currentOwnerId <= 0 || newOwnerId <= 0)
                return BadRequest("Invalid class ID or user IDs.");

            var success = await _participantsService.TransferOwnershipAsync(classId, currentOwnerId, newOwnerId);

            if (!success)
                return BadRequest("Failed to transfer ownership.");

            return Ok("Ownership transferred successfully.");
        }

        // POST: api/classes/{classId}/participants/students/{userId}
        [HttpPost("students/{userId:int}")]
        public async Task<ActionResult<ClassParticipantResponseDto>> AddStudent(int classId, int userId)
        {
            if (classId <= 0 || userId <= 0)
                return BadRequest("Invalid class ID or user ID.");

            var participant = await _participantsService.AddStudentAsync(userId, classId);

            return CreatedAtAction(nameof(GetAllParticipants), new { classId }, participant);
        }

        //Leave from Class
        // DELETE: api/classes/{classId}/participants/students/{userId}
        [HttpDelete("students/{userId:int}")]
        public async Task<IActionResult> RemoveStudent(int classId, int userId)
        {
            if (classId <= 0 || userId <= 0)
                return BadRequest("Invalid class ID or user ID.");

            var success = await _participantsService.RemoveStudentAsync(userId, classId);

            if (!success)
                return BadRequest("Failed to remove student.");

            return NoContent();
        }
    }
}
