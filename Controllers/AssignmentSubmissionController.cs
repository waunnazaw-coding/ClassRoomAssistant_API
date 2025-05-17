using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Implements;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Controllers
{
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AssignmentSubmissionDto>> CreateSubmission(
            int assignmentId,
            [FromForm] AssignmentSubmissionCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (assignmentId != dto.AssignmentId)
                return BadRequest(new { Message = "AssignmentId in URL and body do not match." });

            var result = await _submissionService.CreateSubmissionAsync(dto, cancellationToken);

            return Ok("Successfully");
        }


        [HttpGet("{studentId}/files/{fileName}")]
        public async Task<IActionResult> DownloadSubmissionFile(int assignmentId, int studentId, string fileName, CancellationToken cancellationToken)
        {
            // Construct remote FTP path matching your upload structure
            string remoteFilePath = $"/assignments/{assignmentId}/submissions/{studentId}/{fileName}";

            try
            {
                var fileStream = await _submissionService.DownloadFileFromFtpAsync(remoteFilePath, cancellationToken);

                // Determine content type (optional)
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileName, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(fileStream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                // Log exception as needed
                return StatusCode(500, $"Error downloading file: {ex.Message}");
            }
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
    

    public class UrlSubmissionDto
    {
        public string Link { get; set; } = null!;
        public string ResponseType { get; set; } = null!;
    }
}
