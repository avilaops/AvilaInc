using Manager.Core.Entities.Identity;
using Manager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ManagerDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    public async Task<User?> GetByResetTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken == token && u.ResetTokenExpires > DateTime.UtcNow);
    }

    public async Task<User?> GetByVerifyTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.VerifyToken == token && u.VerifyTokenExpires > DateTime.UtcNow);
    }
}