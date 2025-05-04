using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IAssignmentSubmissionRepository
{
    Task<List<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
    Task<AssignmentSubmission> AddAsync(AssignmentSubmission submission);
}