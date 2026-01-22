using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/funnels")]
[Authorize]
public class FunnelController : ControllerBase
{
    private readonly IMongoCollection<ConversionFunnel> _funnelsCollection;
    private readonly IMongoCollection<FunnelProgress> _progressCollection;
    private readonly IMongoCollection<Session> _sessionsCollection;
    private readonly ILogger<FunnelController> _logger;

    public FunnelController(
        IMongoCollection<ConversionFunnel> funnelsCollection,
        IMongoCollection<FunnelProgress> progressCollection,
        IMongoCollection<Session> sessionsCollection,
        ILogger<FunnelController> logger)
    {
        _funnelsCollection = funnelsCollection;
        _progressCollection = progressCollection;
        _sessionsCollection = sessionsCollection;
        _logger = logger;
    }

    // GET /api/funnels
    [HttpGet]
    public async Task<ActionResult> GetFunnels([FromQuery] string? siteId = null)
    {
        try
        {
            var filter = string.IsNullOrEmpty(siteId)
                ? Builders<ConversionFunnel>.Filter.Empty
                : Builders<ConversionFunnel>.Filter.Eq(f => f.SiteId, siteId);

            var funnels = await _funnelsCollection
                .Find(filter)
                .SortByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(new { success = true, data = funnels });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting funnels");
            return StatusCode(500, new { success = false, error = "Erro ao buscar funis" });
        }
    }

    // POST /api/funnels
    [HttpPost]
    public async Task<ActionResult> CreateFunnel([FromBody] CreateFunnelRequest request)
    {
        try
        {
            var funnel = new ConversionFunnel
            {
                Name = request.Name,
                Description = request.Description,
                SiteId = request.SiteId,
                Steps = request.Steps
            };

            await _funnelsCollection.InsertOneAsync(funnel);

            return Ok(new { success = true, data = funnel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating funnel");
            return StatusCode(500, new { success = false, error = "Erro ao criar funil" });
        }
    }

    // GET /api/funnels/{id}/stats
    [HttpGet("{id}/stats")]
    public async Task<ActionResult> GetFunnelStats(string id, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var funnel = await _funnelsCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (funnel == null)
            {
                return NotFound(new { success = false, error = "Funil não encontrado" });
            }

            var filter = Builders<FunnelProgress>.Filter.Eq(p => p.FunnelId, funnel.FunnelId) &
                        Builders<FunnelProgress>.Filter.Gte(p => p.CreatedAt, startDate) &
                        Builders<FunnelProgress>.Filter.Lte(p => p.CreatedAt, endDate);

            var allProgress = await _progressCollection.Find(filter).ToListAsync();

            var totalStarted = allProgress.Count;
            var totalCompleted = allProgress.Count(p => p.Completed);

            var stepStats = new List<object>();
            for (int i = 0; i < funnel.Steps.Count; i++)
            {
                var stepNumber = i + 1;
                var reachedStep = allProgress.Count(p => p.CurrentStep >= stepNumber || p.Completed);
                var conversionRate = totalStarted > 0 ? (double)reachedStep / totalStarted * 100 : 0;

                stepStats.Add(new
                {
                    stepNumber,
                    name = funnel.Steps[i].Name,
                    reached = reachedStep,
                    conversionRate = Math.Round(conversionRate, 2)
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    funnelName = funnel.Name,
                    totalStarted,
                    totalCompleted,
                    overallConversionRate = totalStarted > 0 ? Math.Round((double)totalCompleted / totalStarted * 100, 2) : 0,
                    steps = stepStats
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting funnel stats");
            return StatusCode(500, new { success = false, error = "Erro ao buscar estatísticas" });
        }
    }

    // POST /api/funnels/track
    [HttpPost("track")]
    [AllowAnonymous]
    public async Task<ActionResult> TrackFunnelProgress([FromBody] TrackFunnelRequest request)
    {
        try
        {
            var funnel = await _funnelsCollection
                .Find(f => f.FunnelId == request.FunnelId && f.Active)
                .FirstOrDefaultAsync();

            if (funnel == null)
            {
                return NotFound(new { success = false, error = "Funil não encontrado" });
            }

            var progress = await _progressCollection
                .Find(p => p.FunnelId == request.FunnelId && p.SessionId == request.SessionId)
                .FirstOrDefaultAsync();

            if (progress == null)
            {
                progress = new FunnelProgress
                {
                    FunnelId = request.FunnelId,
                    SessionId = request.SessionId,
                    VisitorId = request.VisitorId,
                    SiteId = funnel.SiteId,
                    CurrentStep = 1,
                    StepTimestamps = new Dictionary<int, DateTime> { { 1, DateTime.UtcNow } }
                };
                await _progressCollection.InsertOneAsync(progress);
            }

            // Check if current action matches next step
            var nextStep = funnel.Steps.FirstOrDefault(s => s.StepNumber == progress.CurrentStep + 1);
            if (nextStep != null && MatchesStep(nextStep, request))
            {
                progress.CurrentStep++;
                progress.StepTimestamps[progress.CurrentStep] = DateTime.UtcNow;

                if (progress.CurrentStep >= funnel.Steps.Count)
                {
                    progress.Completed = true;
                    progress.CompletedAt = DateTime.UtcNow;

                    // Mark session as converted
                    var session = await _sessionsCollection.Find(s => s.SessionId == request.SessionId).FirstOrDefaultAsync();
                    if (session != null)
                    {
                        session.Converted = true;
                        await _sessionsCollection.ReplaceOneAsync(s => s.SessionId == session.SessionId, session);
                    }
                }

                await _progressCollection.ReplaceOneAsync(p => p.Id == progress.Id, progress);
            }

            return Ok(new { success = true, currentStep = progress.CurrentStep, completed = progress.Completed });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking funnel progress");
            return StatusCode(500, new { success = false, error = "Erro ao rastrear progresso" });
        }
    }

    private bool MatchesStep(FunnelStep step, TrackFunnelRequest request)
    {
        return step.MatchType switch
        {
            "url" => request.Url?.Contains(step.MatchValue, StringComparison.OrdinalIgnoreCase) ?? false,
            "event" => request.EventName == step.MatchValue,
            _ => false
        };
    }
}

public class CreateFunnelRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public List<FunnelStep> Steps { get; set; } = new();
}

public class TrackFunnelRequest
{
    public string FunnelId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string VisitorId { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? EventName { get; set; }
}
