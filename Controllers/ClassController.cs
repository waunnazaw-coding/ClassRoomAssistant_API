using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                // TODO: Replace with actual authenticated user ID
                int userId = 1;

                var createdClass = await _classService.AddClassAsync(classRequestDto, userId);

                return CreatedAtAction(nameof(GetClassById), new { id = createdClass.Id }, createdClass);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding new class", Details = ex.Message });
            }
        }

        // PUT: api/classes/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClassResponseDto>> UpdateClass(int id, [FromBody] ClassRequestDto classRequestDto)
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
    }
}
