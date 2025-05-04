using ClassRoomClone_App.Server.DTOs;

namespace ClassRoomClone_App.Server.Services.Interfaces;

public interface IAssignmentSubmissionService
{
    Task<SubmissionResponseDto> UpdateResponseAsync(
        int assignmentId, 
        int submissionId, 
        int responseId, 
        SubmissionResponseUpdateDto dto
    );
    Task<List<AssignmentSubmissionDto>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
    Task<AssignmentSubmissionDto> CreateSubmissionAsync(AssignmentSubmissionCreateDto dto);
}