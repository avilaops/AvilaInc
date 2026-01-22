using Manager.Core.Entities.Projects;
using Manager.Core.Enums;

namespace Manager.Infrastructure.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status);
    Task<Project?> GetWithSpecAsync(Guid id);
}
