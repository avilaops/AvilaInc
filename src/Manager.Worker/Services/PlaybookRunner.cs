using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Manager.Core.Entities.Orchestration;
using Manager.Core.Enums;
using Manager.Infrastructure.Data;

namespace Manager.Worker.Services;

public class PlaybookRunner : IPlaybookRunner
{
    private readonly ManagerDbContext _context;
    private readonly ILogger<PlaybookRunner> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PlaybookRunner(
        ManagerDbContext context,
        ILogger<PlaybookRunner> logger,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<PlaybookRun> ExecuteAsync(Guid playbookId, Guid projectId, Dictionary<string, object> input)
    {
        var playbook = await _context.Playbooks.FindAsync(playbookId);
        if (playbook == null)
            throw new ArgumentException($"Playbook {playbookId} not found");

        var run = new PlaybookRun
        {
            PlaybookId = playbookId,
            ProjectId = projectId,
            InputJson = JsonSerializer.Serialize(input),
            Status = JobStatus.Running,
            StartedAt = DateTime.UtcNow
        };

        _context.PlaybookRuns.Add(run);
        await _context.SaveChangesAsync();

        try
        {
            var steps = JsonSerializer.Deserialize<List<PlaybookStep>>(playbook.StepsJson);
            if (steps == null || steps.Count == 0)
                throw new InvalidOperationException("Playbook has no steps");

            var context = new Dictionary<string, object>(input);

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var stepRun = new StepRun
                {
                    PlaybookRunId = run.Id,
                    StepName = step.Name,
                    StepOrder = i + 1,
                    InputJson = JsonSerializer.Serialize(context),
                    Status = JobStatus.Running,
                    StartedAt = DateTime.UtcNow
                };

                _context.StepRuns.Add(stepRun);
                await _context.SaveChangesAsync();

                try
                {
                    _logger.LogInformation("Executing step {StepName} for run {RunId}", step.Name, run.Id);

                    // TODO: Execute step handler based on step.Handler
                    // For now, just simulate success
                    await Task.Delay(100);

                    stepRun.Status = JobStatus.Succeeded;
                    stepRun.CompletedAt = DateTime.UtcNow;
                    stepRun.OutputJson = JsonSerializer.Serialize(new { success = true });

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Step {StepName} failed for run {RunId}", step.Name, run.Id);

                    stepRun.Status = JobStatus.Failed;
                    stepRun.CompletedAt = DateTime.UtcNow;
                    stepRun.ErrorMessage = ex.Message;

                    await _context.SaveChangesAsync();

                    throw;
                }
            }

            run.Status = JobStatus.Succeeded;
            run.CompletedAt = DateTime.UtcNow;
            run.OutputJson = JsonSerializer.Serialize(context);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Playbook run {RunId} completed successfully", run.Id);

            return run;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Playbook run {RunId} failed", run.Id);

            run.Status = JobStatus.Failed;
            run.CompletedAt = DateTime.UtcNow;
            run.ErrorMessage = ex.Message;

            await _context.SaveChangesAsync();

            throw;
        }
    }
}

public record PlaybookStep(string Name, string Handler, Dictionary<string, object>? Config);
