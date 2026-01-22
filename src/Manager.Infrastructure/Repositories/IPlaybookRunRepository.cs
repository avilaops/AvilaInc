using Manager.Core.Entities.Orchestration;
using Manager.Core.Enums;

namespace Manager.Infrastructure.Repositories;

public interface IPlaybookRunRepository : IRepository<PlaybookRun>
{
    Task<IEnumerable<PlaybookRun>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<PlaybookRun>> GetByStatusAsync(JobStatus status);
    Task<PlaybookRun?> GetWithStepsAsync(Guid id);
}
