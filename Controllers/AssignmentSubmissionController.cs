using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Controllers;

[ApiController]
[Route("api/assignments/{assignmentId:int}/submissions")]
public class AssignmentSubmissionsController : ControllerBase
{
    private readonly IAssignmentSubmissionService _submissionService;

    public AssignmentSubmissionsController(IAssignmentSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    // GET: api/assignments/{assignmentId}/submissions
    [HttpGet]
    public async Task<ActionResult<List<AssignmentSubmissionDto>>> GetSubmissionsByAssignmentId(int assignmentId)
    {
        var submissions = await _submissionService.GetSubmissionsByAssignmentIdAsync(assignmentId);
        return Ok(submissions);
    }
    

    // POST: api/assignments/{assignmentId}/submissions
    [HttpPost]
    public async Task<ActionResult<AssignmentSubmissionDto>> CreateSubmission(
        int assignmentId,
        [FromBody] AssignmentSubmissionCreateDto dto)
    {
        if (assignmentId != dto.AssignmentId)
            return BadRequest(new { Message = "AssignmentId in URL and body do not match." });

        var result = await _submissionService.CreateSubmissionAsync(dto);

        return CreatedAtRoute("GetSubmissionById", new { assignmentId = result.AssignmentId, submissionId = result.Id }, result);
    }

    // PUT: api/assignments/{assignmentId}/submissions/{submissionId}/responses/{responseId}
    [HttpPut("{submissionId:int}/responses/{responseId:int}")]
    public async Task<ActionResult<SubmissionResponseDto>> UpdateResponse(
        int assignmentId,
        int submissionId,
        int responseId,
        [FromBody] SubmissionResponseUpdateDto dto)
    {
        try
        {
            var updatedResponse = await _submissionService.UpdateResponseAsync(
                assignmentId, submissionId, responseId, dto
            );
            return Ok(updatedResponse);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
