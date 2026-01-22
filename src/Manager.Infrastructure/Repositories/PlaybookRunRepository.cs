using Microsoft.EntityFrameworkCore;
using Manager.Core.Entities.Orchestration;
using Manager.Core.Enums;
using Manager.Infrastructure.Data;

namespace Manager.Infrastructure.Repositories;

public class PlaybookRunRepository : Repository<PlaybookRun>, IPlaybookRunRepository
{
    public PlaybookRunRepository(ManagerDbContext context) : base(context) { }

    public async Task<IEnumerable<PlaybookRun>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlaybookRun>> GetByStatusAsync(JobStatus status)
    {
        return await _dbSet
            .Where(r => r.Status == status)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<PlaybookRun?> GetWithStepsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.StepRuns.OrderBy(s => s.StepOrder))
            .Include(r => r.Playbook)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
