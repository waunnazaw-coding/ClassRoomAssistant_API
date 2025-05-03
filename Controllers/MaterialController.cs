using Microsoft.AspNetCore.Mvc;
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using System;

namespace ClassRoomClone_App.Server.Controllers;

[ApiController]
[Route("api/classes/{classId:int}/materials")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet("{materialId:int}")]
    public async Task<ActionResult<MaterialDetailResponseDto>> GetMaterialDetails(
        [FromRoute] int classId,
        [FromRoute] int materialId)
    {
        try
        {
            var materialDetails = await _materialService.GetMaterialDetailsAsync( materialId);
            return Ok(materialDetails);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<MaterialResponseDto>> CreateMaterial(
        [FromRoute] int classId,
        [FromBody] MaterialCreateRequestDto request)
    {
        if (request.ClassId != classId)
            return BadRequest("Class ID mismatch");

        try
        {
            var result = await _materialService.CreateMaterialWithAttachmentsAsync(request);
            return CreatedAtAction(
                nameof(GetMaterialDetails),
                new { classId, materialId = result.MaterialId },
                result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut("{materialId:int}")]
    public async Task<ActionResult<MaterialResponseDto>> UpdateMaterial(
        [FromRoute] int classId,
        [FromRoute] int materialId,
        [FromQuery] int userId,
        [FromBody] MaterialUpdateRequestDto dto)
    {
        try
        {
            var updated = await _materialService.UpdateMaterialWithAttachmentsAsync(materialId, userId, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("{materialId:int}")]
    public async Task<IActionResult> DeleteMaterial(
        [FromRoute] int classId,
        [FromRoute] int materialId)
    {
        try
        {
            var deleted = await _materialService.DeleteMaterialAsync(materialId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
