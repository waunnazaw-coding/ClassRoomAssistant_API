
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _repository;
    private readonly IToDoRepository _todoRepo;
    private readonly IAttachmentRepository _attachmentRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IClassParticipantsRepository _classParticipantsRepo;
    private readonly IClassWorkRepository _classWorkRepo;
    private readonly DbContextClassName _context;
    private readonly IAssignmentCreateRepository _assignmentCreateRepo;
    
    public AssignmentService(
        IClassWorkRepository classWorkRepo,
        IAssignmentRepository assignmentRepo,
        IToDoRepository todoRepo,
        IAttachmentRepository attachmentRepo,
        INotificationRepository notificationRepo,
        IClassParticipantsRepository classParticipantsRepo,
        DbContextClassName context,
        IAssignmentCreateRepository assignmentCreateRepository)
    {
        _classWorkRepo = classWorkRepo;
        _repository = assignmentRepo;
        _todoRepo = todoRepo;
        _classParticipantsRepo = classParticipantsRepo;
        _context = context;
        _notificationRepo = notificationRepo;
        _assignmentCreateRepo = assignmentCreateRepository;
    }
    public async Task<IEnumerable<AssignmentResponseDto>> GetAllAsync(int classWorkId)
    {
        var assignments = await _repository.GetAllAsync(classWorkId);
        return assignments.Select(MapToResponseDto);
    }

    public async Task<AssignmentResponseDto> GetByIdAsync(int id)
    {
        var assignment = await _repository.GetByIdAsync(id);
        if (assignment == null) throw new KeyNotFoundException("Assignment not found.");
        return MapToResponseDto(assignment);
    }

    
    public async Task<AssignmentCreateResponse> CreateAssignmentAsync(AssignmentCreateRequest request)
    {
        return await _assignmentCreateRepo.CreateFullAssignmentAsync(request);
    }
    

    public async Task<IEnumerable<AssignmentWithStatusDto>> GetAssignmentsWithStatusAsync(int userId)
    {
        var assignments = await _repository.GetAssignmentsWithClassInfoAsync(userId);

        return assignments.Select(a => new AssignmentWithStatusDto
            {
                AssignmentId = a.Id,
                Title = a.Title,
                DueDate = a.DueDate,
                //ClassId = a.ClassWork.ClassId,
                ClassName = a.ClassWork.Class.Name,
                Status = a.ClassWork.Todos
                    .Where(t => t.UserId == userId)
                    .Select(t => t.Status)
                    .FirstOrDefault()
            })
            .OrderBy(a => a.ClassId)
            .ThenBy(a => a.DueDate)
            .ToList();
    }
    
  public async Task<AssignmentResponseDto> CreateAssignmentWithTodosAsync(CreateAssignmentRequestDto request)
{
    if (request == null)
        throw new ArgumentNullException(nameof(request));

    // 1. Handle TopicId: treat 0 or null as null
    int? topicId = (request.TopicId.HasValue && request.TopicId.Value != 0) ? request.TopicId : null;

    // 2. Create ClassWork entity
    var classWork = new ClassWork
    {
        ClassId = request.ClassId,
        TopicId = topicId,
        Type = "Assignment",
        CreatedAt = DateTime.UtcNow
    };

    classWork = await _classWorkRepo.AddClassWorkAsync(classWork)
        ?? throw new NullReferenceException("Failed to create ClassWork.");

    // 3. Create Assignment entity linked to ClassWork
    var assignment = new Assignment
    {
        ClassWorkId = classWork.Id,
        Title = request.Title,
        Instructions = request.Instructions,
        Points = request.Points,
        DueDate = request.DueDate,
        AllowLateSubmission = request.AllowLateSubmission,
        CreatedBy = request.CreatedByUserId
    };

    assignment = await _repository.AddAsync(assignment)
        ?? throw new NullReferenceException("Failed to create Assignment.");

    // 4. Insert Attachments if any
    

    // 5. Get all student UserIds from ClassParticipants for the class
    var studentUserIds = await _classParticipantsRepo.GetStudentUserIdsByClassIdAsync(request.ClassId);
     
    // 6. Create Todos for each student
    var todos = studentUserIds.Select(userId => new Todo
    {
        UserId = userId,
        ClassWorkId = classWork.Id,
        DueDate = request.DueDate,
        Status = "Pending",
        IsMissing = false
    }).ToList();

    if (todos.Any())
        await _todoRepo.BulkAddTodosAsync(todos);

    // 7. Create Notifications for each student
    var notifications = studentUserIds.Select(userId => new Notification
    {
        UserId = userId,
        Type = "Assignment",
        ReferenceId = assignment.Id,
        IsRead = false,
        CreatedAt = DateTime.UtcNow
    }).ToList();

    if (notifications.Any())
        await _notificationRepo.AddRangeAsync(notifications);

    // 8. Return response DTO
    return new AssignmentResponseDto
    {
        Id = assignment.Id,
        ClassWorkId = classWork.Id,
        Title = assignment.Title,
        DueDate = assignment.DueDate
    };
}


    public async Task<AssignmentResponseDto> UpdateAsync(int id, AssignmentUpdateRequestDto dto)
    {
        // Start transaction
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Assignment
            var assignment = await _repository.GetByIdAsync(id);
            if (assignment == null) throw new KeyNotFoundException("Assignment not found.");

            assignment.Title = dto.Title;
            assignment.Instructions = dto.Instructions;
            assignment.Points = dto.Points;
            assignment.DueDate = dto.DueDate;
            assignment.AllowLateSubmission = dto.AllowLateSubmission;

            var updatedAssignment = await _repository.UpdateAsync(assignment);

            // 2. Update related Todos DueDate if DueDate changed
            if (dto.DueDate.HasValue)
            {
                await _todoRepo.UpdateDueDateByClassWorkIdAsync(assignment.ClassWorkId, dto.DueDate.Value);
            }

            await transaction.CommitAsync();

            return MapToResponseDto(updatedAssignment!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var assignment = await _repository.GetByIdAsync(id);
            if (assignment == null) return false;

            var classWorkId = assignment.ClassWorkId;
            
            await _todoRepo.DeleteByClassWorkIdAsync(classWorkId);

            // 3. Delete the assignment
            var deletedAssignment = await _repository.DeleteAsync(id);
            if (!deletedAssignment)
                throw new Exception("Failed to delete assignment.");

            // 4. Optionally delete the ClassWork if no other assignments exist
            var otherAssignments = await _repository.GetAllAsync(classWorkId);
            if (!otherAssignments.Any())
            {
                await _classWorkRepo.DeleteAsync(classWorkId);
            }

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static AssignmentResponseDto MapToResponseDto(Assignment a) => new AssignmentResponseDto
    {
        Id = a.Id,
        ClassWorkId = a.ClassWorkId,
        Title = a.Title,
        Instructions = a.Instructions,
        Points = a.Points,
        DueDate = a.DueDate,
        AllowLateSubmission = a.AllowLateSubmission
    };
}
