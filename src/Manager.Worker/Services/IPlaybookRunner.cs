using Manager.Core.Entities.Orchestration;
using Manager.Core.Enums;

namespace Manager.Worker.Services;

public interface IPlaybookRunner
{
    Task<PlaybookRun> ExecuteAsync(Guid playbookId, Guid projectId, Dictionary<string, object> input);
}
