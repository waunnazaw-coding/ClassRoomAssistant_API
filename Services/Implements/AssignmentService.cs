
using ClassRoomClone_App.Server.DTOs;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Interfaces;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _repository;
    private readonly IToDoRepository _todoRepo;
    private readonly IClassParticipantsRepository _classParticipantsRepo;
    private readonly IClassWorkRepository _classWorkRepo;
    private readonly DbContextClassName _context;
    
    public AssignmentService(
        IClassWorkRepository classWorkRepo,
        IAssignmentRepository assignmentRepo,
        IToDoRepository todoRepo,
        IClassParticipantsRepository classParticipantsRepo,
        DbContextClassName context)
    {
        _classWorkRepo = classWorkRepo;
        _repository = assignmentRepo;
        _todoRepo = todoRepo;
        _classParticipantsRepo = classParticipantsRepo;
        _context = context;
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
        // 1. Create ClassWork
        var classWork = new ClassWork
        {
            ClassId = request.ClassId,
            TopicId = request.TopicId,
            Type = "Assignment",
            CreatedAt = DateTime.UtcNow
        };
        classWork = await _classWorkRepo.AddClassWorkAsync(classWork);

        // 2. Create Assignment linked to ClassWork
        var assignment = new Assignment
        {
            ClassWorkId = classWork.Id,
            Title = request.Title,
            Instructions = request.Instructions,
            Points = request.Points,
            DueDate = request.DueDate
        };
        assignment = await _repository.AddAsync(assignment);

        // 3. Get all student UserIds from ClassParticipants
        var studentUserIds = await _classParticipantsRepo.GetStudentUserIdsByClassIdAsync(request.ClassId);

        // 4. Create Todos for each student
        var todos = studentUserIds.Select(userId => new Todo
        {
            UserId = userId,
            ClassWorkId = classWork.Id,
            DueDate = request.DueDate,
            Status = "Pending",
            IsMissing = false
        }).ToList();

        // 5. Bulk insert Todos
        await _todoRepo.BulkAddTodosAsync(todos);

        // 6. Return response DTO
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
