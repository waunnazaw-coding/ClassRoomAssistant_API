using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Repositories.Implements
{
    public class AssignmentCreateRepository : IAssignmentCreateRepository
    {
        private readonly string _connectionString;

        public AssignmentCreateRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AssignmentCreateResponse> CreateFullAssignmentAsync(AssignmentCreateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.AssignmentTitle))
                throw new ArgumentException("Assignment title is required.", nameof(request.AssignmentTitle));
            if (request.CreateNewTopic && string.IsNullOrWhiteSpace(request.NewTopicTitle))
                throw new ArgumentException("New topic title is required when CreateNewTopic is true.", nameof(request.NewTopicTitle));

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                int? topicId = null;
                if (request.CreateNewTopic)
                {
                    topicId = await CreateTopicAsync( request.NewTopicTitle!, request.ClassId);
                }
                else
                {
                    topicId = request.SelectedTopicId;
                }

                int classWorkId = await CreateClassWorkAsync( request.ClassId, topicId);

                int assignmentId = await CreateAssignmentAsync(
                    classWorkId,
                    request.AssignmentTitle,
                    request.Instructions,
                    request.Points,
                    request.DueDate,
                    request.AllowLateSubmission,
                    request.ClassId);

                if (request.Attachments != null && request.Attachments.Any())
                {
                    await AddAttachmentsAsync( assignmentId, request.Attachments, request.ClassId);
                }

                List<int> students;
                if (request.StudentIds == null || !request.StudentIds.Any())
                {
                    students = (await GetAllStudentIdsInClassAsync( request.ClassId)).ToList();
                }
                else
                {
                    students = request.StudentIds;
                }

                await AddNotificationsAsync( classWorkId, students);
                await AddTodosAsync( classWorkId, students, request.DueDate);

                await transaction.CommitAsync();

                return new AssignmentCreateResponse
                {
                    AssignmentId = assignmentId,
                    ClassWorkId = classWorkId,
                    TopicId = topicId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int?> CreateTopicAsync(string title, int classId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(@"
                INSERT INTO Topics (Title, ClassId) 
                OUTPUT INSERTED.Id 
                VALUES (@Title, @ClassId)", conn);

            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@ClassId", classId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? (int?)Convert.ToInt32(result) : null;
        }

        public async Task<int> CreateClassWorkAsync(int classId, int? topicId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(@"
                INSERT INTO ClassWork (ClassId, TopicId, Type) 
                OUTPUT INSERTED.Id 
                VALUES (@ClassId, @TopicId, 'Assignment')", conn);

            cmd.Parameters.AddWithValue("@ClassId", classId);
            cmd.Parameters.AddWithValue("@TopicId", topicId ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            if (result == null) throw new InvalidOperationException("Failed to create ClassWork.");
            return Convert.ToInt32(result);
        }

        public async Task<int> CreateAssignmentAsync(int classWorkId, string title, string? instructions, int? points, DateTime? dueDate, bool allowLateSubmission, int createdBy)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(@"
                INSERT INTO Assignments 
                    (ClassWorkId, Title, Instructions, Points, DueDate, AllowLateSubmission, CreatedBy) 
                OUTPUT INSERTED.Id
                VALUES 
                    (@ClassWorkId, @Title, @Instructions, @Points, @DueDate, @AllowLateSubmission, @CreatedBy)", conn);

            cmd.Parameters.AddWithValue("@ClassWorkId", classWorkId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Instructions", (object?)instructions ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Points", (object?)points ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DueDate", (object?)dueDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AllowLateSubmission", allowLateSubmission);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            var result = await cmd.ExecuteScalarAsync();
            if (result == null) throw new InvalidOperationException("Failed to create Assignment.");
            return Convert.ToInt32(result);
        }

        public async Task AddAttachmentsAsync(int assignmentId, IEnumerable<AttachmentDto> attachments, int createdByUserId)
        {
            if (attachments == null || !attachments.Any()) return;

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO Attachments 
                    (ReferenceId, ReferenceType, FileType, FileUrl, FilePath, CreatedBy)
                VALUES 
                    (@ReferenceId, 'Assignment', @FileType, @FileUrl, @FilePath, @CreatedBy)";

            foreach (var att in attachments)
            {
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ReferenceId", assignmentId);
                cmd.Parameters.AddWithValue("@FileType", att.FileType);
                cmd.Parameters.AddWithValue("@FileUrl", att.FileUrl ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FilePath", att.FilePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<int>> GetAllStudentIdsInClassAsync(int classId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT UserId 
                FROM ClassParticipants 
                WHERE ClassId = @ClassId 
                    AND Role = 'Student' 
                    AND UserId IS NOT NULL";

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ClassId", classId);

            var userIds = new List<int>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userIds.Add(reader.GetInt32(0));
            }
            return userIds;
        }

        public async Task AddNotificationsAsync(int classWorkId, IEnumerable<int> userIds)
        {
            if (userIds == null || !userIds.Any()) return;

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
    INSERT INTO Notifications 
        (UserId, Type, ReferenceId, IsRead, CreatedAt)
    VALUES 
        (@UserId, 'Assignment', @ReferenceId, 0, GETDATE())";


            foreach (var userId in userIds)
            {
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ReferenceId", classWorkId);


                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task AddTodosAsync(int classWorkId, IEnumerable<int> userIds, DateTime? dueDate)
        {
            if (userIds == null || !userIds.Any()) return;

            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO Todos 
                    (UserId, ClassWorkId, Status, DueDate, IsMissing)
                VALUES 
                    (@UserId, @ClassWorkId, 'Pending', @DueDate, 0)";

            foreach (var userId in userIds)
            {
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ClassWorkId", classWorkId);
                cmd.Parameters.AddWithValue("@DueDate", dueDate ?? (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
