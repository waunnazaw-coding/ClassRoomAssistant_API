using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentResponseDto>> GetAllAsync(int classWorkId);
    Task<AssignmentResponseDto> GetByIdAsync(int id);
    Task<AssignmentCreateResponse> CreateAssignmentAsync(AssignmentCreateRequest request);
    Task<IEnumerable<AssignmentWithStatusDto>> GetAssignmentsWithStatusAsync(int userId);
    Task<AssignmentResponseDto> CreateAssignmentWithTodosAsync(CreateAssignmentRequestDto request);
    Task<AssignmentResponseDto> UpdateAsync(int id, AssignmentUpdateRequestDto dto);
    Task<bool> DeleteAsync(int id);
}