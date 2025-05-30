using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly IAuthorizationService _authorizationService;

    public AssignmentsController(
        IAssignmentService assignmentService,
        IAuthorizationService authorizationService)
    {
        _assignmentService = assignmentService;
        _authorizationService = authorizationService;
    }

    // GET: api/classes/{classId}/assignments/{assignmentId}
    [HttpGet("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentById(
        [FromRoute] int classId,
        [FromRoute] int assignmentId)
    {
        // Allow all roles (Teacher, SubTeacher, Student) to view
        var authResult = await _authorizationService.AuthorizeAsync(
            User, classId, "TeacherOrSubTeacher");

        if (!authResult.Succeeded)
        {
            // Optionally check if student role is allowed for viewing
            var studentAuth = await _authorizationService.AuthorizeAsync(User, classId, "StudentOnly");
            if (!studentAuth.Succeeded)
                return Forbid();
        }

        try
        {
            var assignment = await _assignmentService.GetByIdAsync(assignmentId);
            return Ok(assignment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    // POST: api/classes/{classId}/assignments/create
    [HttpPost("create")]
public async Task<IActionResult> CreateAssignment(
    [FromForm] AssignmentCreateRequest request)
{
    // Authorization check
    var authResult = await _authorizationService.AuthorizeAsync(User, request.ClassId, "TeacherOnly");
    if (!authResult.Succeeded)
        return Forbid();

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _assignmentService.CreateFullAssignmentAsync(request);
    return Ok(result);
}


    [HttpPost]
    public async Task<IActionResult> CreateAssignmentAsync(
        [FromBody] CreateAssignmentRequestDto request)
    {
        // Only Teacher role can create assignments
        var authResult = await _authorizationService.AuthorizeAsync(User, request.ClassId, "TeacherOnly");
        if (!authResult.Succeeded)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _assignmentService.CreateAssignmentWithTodosAsync(request);
        return Ok(result);
    }

    // PUT: api/classes/{classId}/assignments/{assignmentId}
    [HttpPut("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentResponseDto>> UpdateAssignment(
        [FromRoute] int classId,
        [FromRoute] int assignmentId,
        [FromBody] AssignmentUpdateRequestDto dto)
    {
        // Only Teacher role can update
        var authResult = await _authorizationService.AuthorizeAsync(User, classId, "TeacherOnly");
        if (!authResult.Succeeded)
            return Forbid();

        try
        {
            var updated = await _assignmentService.UpdateAsync(assignmentId, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    // DELETE: api/classes/{classId}/assignments/{assignmentId}
    [HttpDelete("{assignmentId:int}")]
    public async Task<IActionResult> DeleteAssignment(
        [FromRoute] int classId,
        [FromRoute] int assignmentId)
    {
        // Only Teacher role can delete
        var authResult = await _authorizationService.AuthorizeAsync(User, classId, "TeacherOnly");
        if (!authResult.Succeeded)
            return Forbid();

        try
        {
            var deleted = await _assignmentService.DeleteAsync(assignmentId);
            if (!deleted) return NotFound();

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "An error occurred while deleting the assignment.");
        }
    }


}
