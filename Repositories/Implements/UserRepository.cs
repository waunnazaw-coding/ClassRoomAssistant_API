using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Repositories.Implements;

public class UserRepository : IUserRepository
{
    private readonly DbContextClassName _context;
    public UserRepository(DbContextClassName context) { _context = context; }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> GetMeAsync(int userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        return user;
    }

}
