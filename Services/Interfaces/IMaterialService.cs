using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IMaterialService
{
    Task<MaterialDetailResponseDto?> GetMaterialDetailsAsync(int materialId);
    Task<MaterialResponseDto> CreateMaterialWithAttachmentsAsync(MaterialCreateRequestDto request);
    Task<MaterialResponseDto> UpdateMaterialWithAttachmentsAsync(int materialId, int userId, MaterialUpdateRequestDto dto);
    Task<bool> DeleteMaterialAsync(int materialId);
}
