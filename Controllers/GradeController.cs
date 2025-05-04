using ClassRoomClone_App.Server.DTOs.GreadeDtos;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers;

[ApiController]
[Route("api/grades")]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpPost]
    public async Task<ActionResult<GradeResponseDto>> CreateGrade([FromBody] GradeCreateRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Grade data is required.");

        try
        {
            var grade = await _gradeService.CreateGradeAsync(dto);
            return grade;
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    [HttpPut("{gradeId}")]
    public async Task<ActionResult<GradeResponseDto>> UpdateGrade(int gradeId, [FromBody] GradeUpdateRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Grade data is required.");

        try
        {
            var updatedGrade = await _gradeService.UpdateGradeAsync(gradeId, dto);
            return Ok(updatedGrade);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}