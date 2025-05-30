using ClassRoomClone_App.Server.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Repositories.Interfaces
{
    public interface IAssignmentCreateRepository
    {
        Task<int?> CreateTopicAsync(string title, int createdByUserId);
        Task<int> CreateClassWorkAsync(int classId, int? topicId);
        Task<int> CreateAssignmentAsync(int classWorkId, string title, string? instructions, int? points, DateTime? dueDate, bool allowLateSubmission, int createdBy);
        Task AddAttachmentsAsync(int assignmentId, IEnumerable<AttachmentDto> attachments, int createdByUserId);
        Task<IEnumerable<int>> GetAllStudentIdsInClassAsync(int classId);
        Task AddNotificationsAsync(int classWorkId, IEnumerable<int> userIds);
        Task AddTodosAsync(int classWorkId, IEnumerable<int> userIds, DateTime? dueDate);

        Task<AssignmentCreateResponse> CreateFullAssignmentAsync(
            AssignmentCreateRequest request);
    }
}