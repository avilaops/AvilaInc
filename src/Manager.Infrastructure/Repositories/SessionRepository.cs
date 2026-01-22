using Manager.Core.Entities.Identity;
using Manager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(ManagerDbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByTokenAsync(string token)
    {
        return await _context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Token == token && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId)
    {
        return await _context.Sessions
            .Where(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeUserSessionsAsync(Guid userId)
    {
        var sessions = await _context.Sessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task CleanupExpiredSessionsAsync()
    {
        var expiredSessions = await _context.Sessions
            .Where(s => s.ExpiresAt <= DateTime.UtcNow || s.IsRevoked)
            .ToListAsync();

        _context.Sessions.RemoveRange(expiredSessions);
        await _context.SaveChangesAsync();
    }
}