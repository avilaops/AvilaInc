using Manager.Core.Entities.Identity;

namespace Manager.Infrastructure.Repositories;

public interface ISessionRepository : IRepository<Session>
{
    Task<Session?> GetByTokenAsync(string token);
    Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId);
    Task RevokeUserSessionsAsync(Guid userId);
    Task CleanupExpiredSessionsAsync();
}