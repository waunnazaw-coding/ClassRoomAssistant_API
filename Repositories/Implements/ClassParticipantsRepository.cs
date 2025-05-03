using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements
{
    public class ClassParticipantsRepository : IClassParticipantsRepository
    {
        private readonly DbContextClassName _context;

        public ClassParticipantsRepository(DbContextClassName context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassParticipant>> GetAllParticipantsAsync(int classId)
        {
            return await _context.ClassParticipants
                .AsNoTracking()
                .Where(cp => cp.ClassId == classId)
                .Include(cp => cp.User)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<int>> GetUserIdsByClassIdAsync(int classId)
        {
            return await _context.ClassParticipants
                .Where(cp => cp.ClassId == classId)
                .Select(cp => cp.UserId.Value)
                .ToListAsync();
        }
        
        public async Task<List<int>> GetStudentUserIdsByClassIdAsync(int classId)
        {
            return await _context.ClassParticipants
                .Where(cp => cp.ClassId == classId && cp.Role == "Student" && cp.UserId != null)
                .Select(cp => cp.UserId.Value)
                .ToListAsync();
        }

        public async Task<ClassParticipant> SetMainTeacherAsync(int userId, int classId)
        {
            if (classId <= 0 || userId <= 0)
                throw new ArgumentException("Class ID and User ID must be greater than zero.");

            var entity = CreateClassParticipant(userId, classId, "Teacher", true);

            await _context.ClassParticipants.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<ClassParticipant> AddSubTeacherAsync(int userId, int classId)
        {
            return await AddParticipantAsync(userId, classId, "SubTeacher", false);
        }

        public async Task<ClassParticipant> AddStudentAsync(int userId, int classId)
        {
            return await AddParticipantAsync(userId, classId, "Student", false);
        }

        public async Task<bool> RemoveStudentAsync(int userId, int classId)
        {
            return await RemoveParticipantAsync(userId, classId, "Student");
        }

        public async Task<bool> RemoveSubTeacherAsync(int userId, int classId)
        {
            return await RemoveParticipantAsync(userId, classId, "SubTeacher");
        }

        public async Task<bool> TransferOwnershipAsync(int classId, int currentOwnerId, int newOwnerId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var currentOwner = await _context.ClassParticipants
                    .FirstOrDefaultAsync(cp => cp.ClassId == classId && cp.UserId == currentOwnerId && cp.IsOwner == true);

                if (currentOwner == null)
                    throw new InvalidOperationException("Current owner not found or not the owner.");

                var newOwner = await _context.ClassParticipants
                    .FirstOrDefaultAsync(cp => cp.ClassId == classId && cp.UserId == newOwnerId && cp.Role == "SubTeacher");

                if (newOwner == null)
                    throw new InvalidOperationException("New owner must be a SubTeacher in the class.");

                currentOwner.IsOwner = false;
                currentOwner.Role = "SubTeacher";

                newOwner.IsOwner = true;
                newOwner.Role = "Teacher";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static ClassParticipant CreateClassParticipant(int userId, int classId, string role, bool isOwner)
        {
            return new ClassParticipant
            {
                UserId = userId,
                ClassId = classId,
                Role = role,
                IsOwner = isOwner,
                AddedAt = DateTime.UtcNow
            };
        }

        private async Task<ClassParticipant> AddParticipantAsync(int userId, int classId, string role, bool isOwner)
        {
            var exists = await _context.ClassParticipants.AnyAsync(cp => cp.ClassId == classId && cp.UserId == userId);
            if (exists)
                throw new Exception("User is already a participant in the class.");

            var entity = CreateClassParticipant(userId, classId, role, isOwner);
            await _context.ClassParticipants.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        private async Task<bool> RemoveParticipantAsync(int userId, int classId, string role)
        {
            var entity = await _context.ClassParticipants
                .FirstOrDefaultAsync(cp => cp.ClassId == classId && cp.UserId == userId && cp.Role == role);

            if (entity == null)
                throw new InvalidOperationException($"{role} not found in the class.");

            _context.ClassParticipants.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
