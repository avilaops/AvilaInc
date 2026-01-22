using Microsoft.EntityFrameworkCore;
using Manager.Core.Entities.Projects;
using Manager.Core.Enums;
using Manager.Infrastructure.Data;

namespace Manager.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ManagerDbContext context) : base(context) { }

    public async Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetWithSpecAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Spec)
            .Include(p => p.Deployments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
