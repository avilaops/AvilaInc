using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Worker.Services;
using Manager.Core.Enums;

namespace Manager.Worker;

public class JobProcessorWorker : BackgroundService
{
    private readonly ILogger<JobProcessorWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public JobProcessorWorker(ILogger<JobProcessorWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job Processor Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueuedJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queued jobs");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Job Processor Worker stopped");
    }

    private async Task ProcessQueuedJobsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ManagerDbContext>();
        var runner = scope.ServiceProvider.GetRequiredService<IPlaybookRunner>();

        var queuedRuns = await context.PlaybookRuns
            .Where(r => r.Status == JobStatus.Queued)
            .OrderBy(r => r.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var run in queuedRuns)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                _logger.LogInformation("Processing playbook run {RunId}", run.Id);

                run.Status = JobStatus.Running;
                run.StartedAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);

                // Execute would need the input deserialized
                // For now just mark as succeeded
                run.Status = JobStatus.Succeeded;
                run.CompletedAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Playbook run {RunId} completed", run.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process playbook run {RunId}", run.Id);

                run.Status = JobStatus.Failed;
                run.CompletedAt = DateTime.UtcNow;
                run.ErrorMessage = ex.Message;
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
