using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/heatmaps")]
public class HeatmapController : ControllerBase
{
    private readonly IMongoCollection<HeatmapClick> _clicksCollection;
    private readonly IMongoCollection<HeatmapScroll> _scrollCollection;
    private readonly IMongoCollection<HeatmapMove> _movesCollection;
    private readonly ILogger<HeatmapController> _logger;

    public HeatmapController(
        IMongoCollection<HeatmapClick> clicksCollection,
        IMongoCollection<HeatmapScroll> scrollCollection,
        IMongoCollection<HeatmapMove> movesCollection,
        ILogger<HeatmapController> logger)
    {
        _clicksCollection = clicksCollection;
        _scrollCollection = scrollCollection;
        _movesCollection = movesCollection;
        _logger = logger;
    }

    // POST /api/heatmaps/track/click
    [HttpPost("track/click")]
    [AllowAnonymous]
    public async Task<ActionResult> TrackClick([FromBody] TrackClickRequest request)
    {
        try
        {
            var click = new HeatmapClick
            {
                SiteId = request.SiteId,
                SessionId = request.SessionId,
                VisitorId = request.VisitorId,
                Url = request.Url,
                Path = new Uri(request.Url).AbsolutePath,
                X = request.X,
                Y = request.Y,
                ScreenWidth = request.ScreenWidth,
                ScreenHeight = request.ScreenHeight,
                ElementTag = request.ElementTag,
                ElementId = request.ElementId,
                ElementClass = request.ElementClass
            };

            await _clicksCollection.InsertOneAsync(click);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking click");
            return StatusCode(500, new { success = false, error = "Erro ao registrar clique" });
        }
    }

    // POST /api/heatmaps/track/scroll
    [HttpPost("track/scroll")]
    [AllowAnonymous]
    public async Task<ActionResult> TrackScroll([FromBody] TrackScrollRequest request)
    {
        try
        {
            var scroll = new HeatmapScroll
            {
                SiteId = request.SiteId,
                SessionId = request.SessionId,
                VisitorId = request.VisitorId,
                Url = request.Url,
                Path = new Uri(request.Url).AbsolutePath,
                MaxDepth = request.MaxDepth
            };

            await _scrollCollection.InsertOneAsync(scroll);

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking scroll");
            return StatusCode(500, new { success = false, error = "Erro ao registrar scroll" });
        }
    }

    // GET /api/heatmaps/clicks/{siteId}
    [HttpGet("clicks/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetClicks(string siteId, [FromQuery] string? path = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-7);
            endDate ??= DateTime.UtcNow;

            var filterBuilder = Builders<HeatmapClick>.Filter;
            var filter = filterBuilder.Eq(c => c.SiteId, siteId) &
                        filterBuilder.Gte(c => c.Timestamp, startDate) &
                        filterBuilder.Lte(c => c.Timestamp, endDate);

            if (!string.IsNullOrEmpty(path))
            {
                filter &= filterBuilder.Eq(c => c.Path, path);
            }

            var clicks = await _clicksCollection.Find(filter).ToListAsync();

            // Normalize coordinates to percentage (0-100) for responsive heatmap
            var normalizedClicks = clicks.Select(c => new
            {
                x = c.ScreenWidth > 0 ? (double)c.X / c.ScreenWidth * 100 : 0,
                y = c.ScreenHeight > 0 ? (double)c.Y / c.ScreenHeight * 100 : 0,
                path = c.Path,
                timestamp = c.Timestamp
            }).ToList();

            return Ok(new { success = true, data = normalizedClicks });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clicks");
            return StatusCode(500, new { success = false, error = "Erro ao buscar cliques" });
        }
    }

    // GET /api/heatmaps/scroll-depth/{siteId}
    [HttpGet("scroll-depth/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetScrollDepth(string siteId, [FromQuery] string? path = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-7);
            endDate ??= DateTime.UtcNow;

            var filterBuilder = Builders<HeatmapScroll>.Filter;
            var filter = filterBuilder.Eq(s => s.SiteId, siteId) &
                        filterBuilder.Gte(s => s.Timestamp, startDate) &
                        filterBuilder.Lte(s => s.Timestamp, endDate);

            if (!string.IsNullOrEmpty(path))
            {
                filter &= filterBuilder.Eq(s => s.Path, path);
            }

            var scrolls = await _scrollCollection.Find(filter).ToListAsync();

            var avgDepth = scrolls.Any() ? scrolls.Average(s => s.MaxDepth) : 0;
            
            var depthRanges = new
            {
                depth_0_25 = scrolls.Count(s => s.MaxDepth <= 25),
                depth_25_50 = scrolls.Count(s => s.MaxDepth > 25 && s.MaxDepth <= 50),
                depth_50_75 = scrolls.Count(s => s.MaxDepth > 50 && s.MaxDepth <= 75),
                depth_75_100 = scrolls.Count(s => s.MaxDepth > 75)
            };

            return Ok(new
            {
                success = true,
                data = new
                {
                    avgDepth = Math.Round(avgDepth, 2),
                    ranges = depthRanges,
                    total = scrolls.Count
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scroll depth");
            return StatusCode(500, new { success = false, error = "Erro ao buscar profundidade de scroll" });
        }
    }
}

public class TrackClickRequest
{
    public string SiteId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string VisitorId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public string? ElementTag { get; set; }
    public string? ElementId { get; set; }
    public string? ElementClass { get; set; }
}

public class TrackScrollRequest
{
    public string SiteId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string VisitorId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int MaxDepth { get; set; }
}
