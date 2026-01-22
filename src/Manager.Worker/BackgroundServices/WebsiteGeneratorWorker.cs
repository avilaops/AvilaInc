using Manager.Core.Enums;
using Manager.Infrastructure.Repositories;
using Manager.Worker.Services;

namespace Manager.Worker.BackgroundServices;

public sealed class WebsiteGeneratorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WebsiteGeneratorWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(10);

    public WebsiteGeneratorWorker(
        IServiceProvider serviceProvider,
        ILogger<WebsiteGeneratorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebsiteGeneratorWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var requestRepo = scope.ServiceProvider.GetRequiredService<IWebsiteRequestRepository>();
                var generatorService = scope.ServiceProvider.GetRequiredService<IWebsiteGeneratorService>();

                // Buscar pedidos pendentes
                var pendingRequests = await requestRepo.GetByStatusAsync(WebsiteRequestStatus.Received);

                foreach (var request in pendingRequests.Take(5)) // Processar 5 por vez
                {
                    _logger.LogInformation("Processing website request {RequestId}", request.Id);

                    try
                    {
                        var project = await generatorService.GenerateWebsiteAsync(request);
                        
                        if (project != null)
                        {
                            _logger.LogInformation(
                                "Website generated successfully for {RequestId}: {PreviewUrl}",
                                request.Id, project.PreviewUrl);
                        }
                        else
                        {
                            _logger.LogWarning("Website generation failed for {RequestId}", request.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing website request {RequestId}", request.Id);
                    }
                }

                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WebsiteGeneratorWorker");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        _logger.LogInformation("WebsiteGeneratorWorker stopped");
    }
}
