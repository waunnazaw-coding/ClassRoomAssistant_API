using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Helpers;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<ApiResponse<IEnumerable<ClassResponseDto>>>> GetAllClasses()
        {
            var classes = await _classService.GetAllClassesAsync();
            return Ok(new ApiResponse<IEnumerable<ClassResponseDto>>(classes, true, "Classes retrieved successfully."));
        }

        // GET: api/classes/user/userId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserClassesRawDto>>> GetUserClasses(int userId)
        {
            var classes = await _classService.GetClassesByUserId(userId);

            if (classes == null || !classes.Any())
            {
                // Return 404 Not Found if no classes found for the user
                return NotFound("No classes found for the specified user.");
            }

            // Return 200 OK with the list of classes
            return Ok(classes);
        }


        // GET: api/classes/archived
        [HttpGet("archived")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClassResponseDto>>>> GetArchivedClasses()
        {
            var archivedClasses = await _classService.GetArchivedClassesAsync();

            if (archivedClasses == null || !archivedClasses.Any())
                return NotFound(new ApiResponse<IEnumerable<ClassResponseDto>>(Enumerable.Empty<ClassResponseDto>(), false, "No archived classes found."));

            return Ok(new ApiResponse<IEnumerable<ClassResponseDto>>(archivedClasses, true, "Archived classes retrieved successfully."));
        }

        // GET: api/classes/{id}/details
        [HttpGet("{id:int}/details")]
        public async Task<ActionResult<ApiResponse<GetClassDetailsResponse>>> GetClassDetails(int id)
        {
            try
            {
                var classDetails = await _classService.GetClassDetailsAsync(id);
                return Ok(new ApiResponse<GetClassDetailsResponse>(classDetails, true, "Class details retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<GetClassDetailsResponse>((GetClassDetailsResponse)null, false, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<GetClassDetailsResponse>((GetClassDetailsResponse)null, false, "An unexpected error occurred."));
            }
        }

        // GET: api/classes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassResponseDto>> GetClassById(int id)
        {
            try
            {
                var classDto = await _classService.GetClassByIdAsync(id);

                if (classDto == null)
                {
                    // Return 404 if the class was not found
                    return NotFound("Class not found.");
                }

                // Return 200 OK with the class data
                return Ok(classDto);
            }
            catch (KeyNotFoundException ex)
            {
                // Return 404 Not Found with message
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return 500 Internal Server Error with generic message
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // POST: api/classes
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClassResponseDto>>> AddClass([FromBody] ClassRequestDto classRequestDto)
        {
            if (classRequestDto == null)
                return BadRequest(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, "Class data is required."));

            try
            {
                var createdClass = await _classService.AddClassAsync(classRequestDto, classRequestDto.UserId);
                return CreatedAtAction(nameof(GetClassById), new { id = createdClass.Id }, new ApiResponse<ClassResponseDto>(createdClass, true, "Class created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, $"Error adding new class: {ex.Message}"));
            }
        }

        // GET api/classes/code/{classCode}
        [HttpGet("code/{classCode}")]
        public async Task<ActionResult<ApiResponse<ClassResponseDto>>> GetClassByCode(string classCode)
        {
            var cls = await _classService.GetClassByCodeAsync(classCode);
            if (cls == null)
                return NotFound(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, "Class not found."));

            return Ok(new ApiResponse<ClassResponseDto>(cls, true, "Class retrieved successfully."));
        }

        // POST api/classes/code/{classCode}/enroll/{studentId}
        [HttpPost("code/{classCode}/enroll/{studentId}")]
        public async Task<ActionResult<ApiResponse<object>>> EnrollStudent(string classCode, int studentId)
        {
            var success = await _classService.EnrollStudentInClassAsync(classCode, studentId);
            if (!success)
                return BadRequest(new ApiResponse<object>(null, false, "Enrollment failed: class not found or student already enrolled."));

            return Ok(new ApiResponse<object>(null, true, "Student enrolled successfully."));
        }

        // PUT: api/classes/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<ClassResponseDto>>> UpdateClass(int id, [FromBody] ClassUpdateRequestDto classRequestDto)
        {
            if (classRequestDto == null)
                return BadRequest(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, "Class data is required."));

            try
            {
                var updatedClass = await _classService.UpdateClassAsync(id, classRequestDto);
                return Ok(new ApiResponse<ClassResponseDto>(updatedClass, true, "Class updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ClassResponseDto>((ClassResponseDto)null, false, $"Error updating class: {ex.Message}"));
            }
        }

        // DELETE: api/classes/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteClass(int id)
        {
            try
            {
                var success = await _classService.DeleteAsync(id);
                if (success)
                    return Ok(new ApiResponse<object>(null, true, "Class deleted successfully."));

                return NotFound(new ApiResponse<object>(null, false, "Class not found."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(null, false, $"Error deleting class: {ex.Message}"));
            }
        }

        // POST: api/classes/{id}/restore
        [HttpPost("{id:int}/restore")]
        public async Task<ActionResult<ApiResponse<object>>> RestoreClass(int id)
        {
            try
            {
                var success = await _classService.RestoreAsync(id);
                if (success)
                    return Ok(new ApiResponse<object>(null, true, "Class restored successfully."));

                return NotFound(new ApiResponse<object>(null, false, "Class not found."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(null, false, $"Error restoring class: {ex.Message}"));
            }
        }

        // DELETE: api/classes/{id}/actual-delete
        [HttpDelete("{id:int}/actual-delete")]
        public async Task<ActionResult<ApiResponse<object>>> ActualDeleteClass(int id)
        {
            try
            {
                var success = await _classService.ActualDeleteAsync(id);
                if (success)
                    return Ok(new ApiResponse<object>(null, true, "Class permanently deleted successfully."));

                return NotFound(new ApiResponse<object>(null, false, "Class not found."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(null, false, $"Error deleting class permanently: {ex.Message}"));
            }
        }
    }
}
