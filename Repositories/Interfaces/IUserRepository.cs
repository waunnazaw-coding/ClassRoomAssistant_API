using ClassRoomClone_App.Server.Models;

namespace ClassRoomClone_App.Server.Repositories.Interfaces;

public interface IUserRepository
{
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User> GetMeAsync(int userId);
}