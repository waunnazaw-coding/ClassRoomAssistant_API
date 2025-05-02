using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Repositories.Implements
{
    public class ClassRepository : IClassRepository
    {
        private readonly DbContextClassName _context;

        public ClassRepository(DbContextClassName context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _context.Classes
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Class>> GetArchivedClassesAsync()
        {
            return await _context.Classes
                .AsNoTracking()
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Class?> GetClassByIdAsync(int id)
        {
            return await _context.Classes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Class?> GetClassDetailsAsync(int id)
        {
            return await _context.Classes
                .AsNoTracking()
                .Include(c => c.ClassParticipants)
                    .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Class> AddClassAsync(Class entity)
        {
            await _context.Classes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Class?> UpdateClassAsync(Class entity)
        {
            var existing = await _context.Classes.FindAsync(entity.Id);
            if (existing == null || existing.IsDeleted)
                return null;

            existing.Name = entity.Name;
            existing.Section = entity.Section;
            existing.Subject = entity.Subject;
            existing.Room = entity.Room;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Classes.FindAsync(id);
            if (entity == null || entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
