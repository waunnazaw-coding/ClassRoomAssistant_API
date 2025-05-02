using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using System.Threading.Tasks;

[ApiController]
[Route("api/classes/{classId:int}/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    // GET: api/classes/{classId}/assignments/{assignmentId}
    [HttpGet("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentResponseDto>> GetAssignmentById(
        [FromRoute] int classId,
        [FromRoute] int assignmentId)
    {
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

    // POST: api/classes/{classId}/assignments
    [HttpPost]
    public async Task<ActionResult<AssignmentResponseDto>> CreateAssignment(
        [FromRoute] int classId,
        [FromBody] CreateAssignmentRequestDto dto)
    {
        if (dto.ClassId != classId)
            return BadRequest("Class ID mismatch.");

        var created = await _assignmentService.CreateAssignmentWithTodosAsync(dto);

        return CreatedAtAction(
            nameof(GetAssignmentById),
            new { classId, assignmentId = created.Id },
            created);
    }

    // PUT: api/classes/{classId}/assignments/{assignmentId}
    [HttpPut("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentResponseDto>> UpdateAssignment(
        [FromRoute] int classId,
        [FromRoute] int assignmentId,
        [FromBody] AssignmentUpdateRequestDto dto)
    {
        try
        {
            var updated = await _assignmentService.UpdateAsync(assignmentId, dto);
            // Optional: Validate updated assignment belongs to classId here or in service

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
