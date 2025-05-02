using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassRoomClone_App.Server.Controllers;

[ApiController]
[Route("api/materials")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet("{materialId:int}/details")]
    public async Task<ActionResult<MaterialDetailResponseDto>> GetMaterialDetails(int materialId)
    {
        var materialDetails = await _materialService.GetMaterialDetailsAsync(materialId);

        if (materialDetails == null)
        {
            return NotFound();
        }

        return Ok(materialDetails);
    }
    
    
    [HttpPost]
    public async Task<ActionResult<MaterialResponseDto>> CreateMaterial(
        [FromBody] MaterialCreateRequestDto request)
    {
        try
        {
            var result = await _materialService.CreateMaterialWithAttachmentsAsync(request);
            return result;
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
    
    
    [HttpPut("{materialId:int}")]
    public async Task<ActionResult<MaterialResponseDto>> UpdateMaterial(
        int materialId,
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
    public async Task<IActionResult> DeleteMaterial(int materialId)
    {
        var deleted = await _materialService.DeleteMaterialAsync(materialId);
        if (!deleted) return NotFound();
        return NoContent();
    }


}
