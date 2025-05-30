using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassRoomClone_App.Server.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ClassRoomClone_App.Server.Repositories.Implements
{
    public class ClassRepository : IClassRepository
    {
        private readonly string _connectionString ;
        private readonly DbContextClassName _context;

        public ClassRepository(DbContextClassName context , IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _context.Classes
                .AsNoTracking()
                .Where(c => c.IsDeleted == false)
                .ToListAsync();
        }
        
        public async Task<bool> ApproveParticipantAsync(int userId, int classId)
        {
            const string query = @"
                                    UPDATE ClassParticipants
                                    SET Status = 'Approved'
                                    WHERE UserId = @UserId
                                      AND ClassId = @ClassId
                                      AND Status = 'Pending';
                                ";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@ClassId", SqlDbType.Int).Value = classId;

                await connection.OpenAsync();

                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Return true if at least one row was updated
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<UserClassesRawDto>> GetClassesByUserId(int userId)
        {
            return await _context.UserClassesRawDtos
                .FromSqlInterpolated($@"EXEC GetClassesUserId @UserId  = {userId}")
                .ToListAsync();
        }

        public async Task<IEnumerable<ClassDetailsWithEntityId>> GetClassDetailsWithEntityIdAsync(int classId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ClassId", classId, DbType.Int32);

                // Call stored procedure
                var result = await db.QueryAsync<ClassDetailsWithEntityId>(
                    "dbo.[GetClassDetailsWithEntityId]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<IEnumerable<Class>> GetArchivedClassesAsync()
        {
            return await _context.Classes
                .AsNoTracking()
                .Where(c => c.IsDeleted == true)
                .ToListAsync();
        }

        public async Task<Class?> GetClassByIdAsync(int id)
        {
            return await _context.Classes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }

        public async Task<List<ClassDetailDto>> GetClassDetailsAsync(int classId)
        {
            var results = new List<ClassDetailDto>();

            using (var conn = new SqlConnection(_connectionString)) 
            using (var cmd = new SqlCommand("GetClassDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ClassId", SqlDbType.Int) { Value = classId });

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var detail = new ClassDetailDto
                        {
                            EntityId = reader.GetInt32(reader.GetOrdinal("EntityId")),
                            EntityType = reader.GetString(reader.GetOrdinal("EntityType")),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString(reader.GetOrdinal("Content")),
                            ActivityDate = reader.GetDateTime(reader.GetOrdinal("ActivityDate")),
                            MessageId = reader.IsDBNull(reader.GetOrdinal("MessageId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("MessageId")),
                            SenderId = reader.IsDBNull(reader.GetOrdinal("SenderId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SenderId")),
                            ReceiverId = reader.IsDBNull(reader.GetOrdinal("ReceiverId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ReceiverId")),
                            MessageContent = reader.IsDBNull(reader.GetOrdinal("MessageContent")) ? null : reader.GetString(reader.GetOrdinal("MessageContent")),
                            MessageCreatedAt = reader.IsDBNull(reader.GetOrdinal("MessageCreatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("MessageCreatedAt"))
                        };

                        results.Add(detail);
                    }
                }
                
                conn.Close();
            }

            return results;
        
        }

        public async Task<Class> AddClassAsync(Class entity)
        {
            await _context.Classes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        
        public async Task<bool> ClassCodeExistsAsync(string classCode)
        {
            return await _context.Classes
                .AsNoTracking()
                .AnyAsync(c => c.ClassCode == classCode);
        }
        
        public async Task<Class?> GetClassByCodeAsync(string classCode)
        {
            return await _context.Classes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClassCode == classCode);
        }

        public async Task<bool> StudentExistsInClassAsync(int classId, int studentId)
        {
            return await _context.ClassParticipants
                .AsNoTracking()
                .AnyAsync(cp => cp.ClassId == classId && cp.UserId == studentId);
        }
        
        public async Task AddClassParticipantAsync(ClassParticipant participant)
        {
            await _context.ClassParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<Class?> UpdateClassAsync(Class entity)
        {
            var existing = await _context.Classes.FindAsync(entity.Id);
            if (existing == null || existing.IsDeleted == true)
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
            if (entity == null || entity.IsDeleted == true)
                return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> RestoreAsync(int id)
        {
            var entity = await _context.Classes.FindAsync(id);
            if (entity == null )
                return false;

            entity.IsDeleted = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> ActualDeleteAsync(int id)
        {
            var entity = await _context.Classes.FindAsync(id);
            if (entity == null )
                return false;

             _context.Classes.Remove(entity);
             
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
