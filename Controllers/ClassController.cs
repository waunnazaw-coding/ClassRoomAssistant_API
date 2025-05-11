using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Controllers
{
    [Route("api/classes")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        // GET: api/classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassResponseDto>>> GetAllClasses()
        {
            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }

        // GET: api/classes/user/userId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserClassesRawDto>>> GetUserClasses(int userId)
        {
            var classes = await _classService.GetClassesByUserId(userId);
            return Ok(classes);
        }
        
        // GET: api/classes/archived
        [HttpGet("archived")]
        public async Task<ActionResult<IEnumerable<ClassResponseDto>>> GetArchivedClasses()
        {
            var archivedClasses = await _classService.GetArchivedClassesAsync();

            if (archivedClasses == null || !archivedClasses.Any())
                return NotFound("No archived classes found.");

            return Ok(archivedClasses);
        }

        
        // GET: api/classes/{id}/details
        [HttpGet("{id:int}/details")]
        public async Task<ActionResult<ClassDetailsResponseDto>> GetClassDetails(int id)
        {
            try
            {
                var classDetails = await _classService.GetClassDetailsAsync(id);
                return Ok(classDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        
        // GET: api/classes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassResponseDto>> GetClassById(int id)
        {
            try
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                return Ok(classDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        
        // POST: api/classes
        [HttpPost]
        public async Task<ActionResult<ClassResponseDto>> AddClass([FromBody] ClassRequestDto classRequestDto)
        {
            if (classRequestDto == null)
                return BadRequest("Class data is required.");

            try
            {
                var createdClass = await _classService.AddClassAsync(classRequestDto, classRequestDto.UserId);

                return CreatedAtAction(nameof(GetClassById), new { id = createdClass.Id }, createdClass);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding new class", Details = ex.Message });
            }
        }
        
        
        // GET api/classes/code/{classCode}
        [HttpGet("code/{classCode}")]
        public async Task<ActionResult<ClassResponseDto>> GetClassByCode(string classCode)
        {
            var cls = await _classService.GetClassByCodeAsync(classCode);
            if (cls == null) return NotFound();

            return Ok(cls);
        }

        
        // POST api/classes/code/{classCode}/enroll/{studentId}
        [HttpPost("code/{classCode}/enroll/{studentId}")]
        public async Task<IActionResult> EnrollStudent(string classCode, int studentId)
        {
            var success = await _classService.EnrollStudentInClassAsync(classCode, studentId);
            if (!success) return BadRequest("Enrollment failed: class not found or student already enrolled.");

            return Ok(new { Message = "Student enrolled successfully." });
        }

        
        // PUT: api/classes/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClassResponseDto>> UpdateClass(int id, [FromBody] ClassUpdateRequestDto classRequestDto)
        {
            if (classRequestDto == null)
                return BadRequest("Class data is required.");

            try
            {
                var updatedClass = await _classService.UpdateClassAsync(id, classRequestDto);
                return Ok(updatedClass);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating class", Details = ex.Message });
            }
        }

        
        // DELETE: api/classes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                var success = await _classService.DeleteAsync(id);
                if (success)
                    return NoContent();

                return NotFound(new { Message = "Class not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting class", Details = ex.Message });
            }
        }
        
        // DELETE: api/classes/{id}
        [HttpPost("{id:int}/restore")]
        public async Task<IActionResult> RestoreClass(int id)
        {
            try
            {
                var success = await _classService.RestoreAsync(id);
                if (success)
                    return NoContent();

                return NotFound(new { Message = "Class not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting class", Details = ex.Message });
            }
        }
        
        // DELETE: api/classes/{id}
        [HttpDelete("{id:int}/actual-delete")]
        public async Task<IActionResult> ActualDeleteClass(int id)
        {
            try
            {
                var success = await _classService.ActualDeleteAsync(id);
                if (success)
                    return NoContent();

                return NotFound(new { Message = "Class not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting class", Details = ex.Message });
            }
        }
    }
}
