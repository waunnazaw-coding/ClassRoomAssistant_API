using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ClassRoomClone_App.Server.Controllers
{
    [ApiController]
    [Route("api/classes/{classId:int}/participants")]
    public class ClassParticipantsController : ControllerBase
    {
        private readonly IClassParticipantsService _participantsService;
        private readonly IUserContextService _userContext;
        private readonly IClassService _classService;

        public ClassParticipantsController(IClassParticipantsService participantsService , IClassService classService , IUserContextService userContext)
        {
            _participantsService = participantsService;
            _userContext = userContext;
            _classService = classService;
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
        
        [HttpPost("subteachers")]
        public async Task<IActionResult> AddSubTeacher(int teacherUserId , int classId, [FromBody] AddParticipantDto dto)
        {
            //var teacherUserId = _userContext.GetCurrentUserId();

            await _participantsService.AddSubTeacherToClassAsync(teacherUserId, classId, dto.Email);

            return Ok(new { message = "SubTeacher added successfully." });
        }
        
        [HttpPost("students")]
        public async Task<IActionResult> AddStudent(int teacherUserId, int classId, [FromBody] AddParticipantDto dto)
        {
            //var teacherUserId = _userContext.GetCurrentUserId();

            await _participantsService.AddStudentToClassAsync(teacherUserId, classId, dto.Email);

            return Ok(new { message = "Student added successfully." });
        }
        
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveParticipant(int userId, int classId)
        {
            if (userId <= 0 || classId <= 0)
            {
                return BadRequest(new { message = "Invalid userId or classId." });
            }

            bool success = await _classService.ApproveParticipantAsync(userId, classId);

            if (success)
            {
                return Ok(new { message = "Participant approved successfully." });
            }
            else
            {
                return NotFound(new { message = "Participant not found or already approved." });
            }
        }
        
        public class AddParticipantDto
        {
            public string Email { get; set; }
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
