using Manager.Core.Entities.Identity;

namespace Manager.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateLastLoginAsync(Guid userId);
    Task<User?> GetByResetTokenAsync(string token);
    Task<User?> GetByVerifyTokenAsync(string token);
}