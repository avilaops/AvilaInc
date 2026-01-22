using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;
using System.Text.Json;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IMongoCollection<PageView> _pageviewsCollection;
    private readonly IMongoCollection<AnalyticsEvent> _eventsCollection;
    private readonly IMongoCollection<Session> _sessionsCollection;
    private readonly IMongoCollection<Visitor> _visitorsCollection;
    private readonly IMongoCollection<Site> _sitesCollection;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IMongoCollection<PageView> pageviewsCollection,
        IMongoCollection<AnalyticsEvent> eventsCollection,
        IMongoCollection<Session> sessionsCollection,
        IMongoCollection<Visitor> visitorsCollection,
        IMongoCollection<Site> sitesCollection,
        ILogger<AnalyticsController> logger)
    {
        _pageviewsCollection = pageviewsCollection;
        _eventsCollection = eventsCollection;
        _sessionsCollection = sessionsCollection;
        _visitorsCollection = visitorsCollection;
        _sitesCollection = sitesCollection;
        _logger = logger;
    }

    // POST /api/analytics/track/pageview
    [HttpPost("track/pageview")]
    [AllowAnonymous]
    public async Task<ActionResult> TrackPageview([FromBody] TrackPageviewRequest request)
    {
        try
        {
            // Validate site
            var site = await _sitesCollection.Find(s => s.SiteId == request.SiteId && s.Active).FirstOrDefaultAsync();
            if (site == null)
            {
                return BadRequest(new { success = false, error = "Site inválido" });
            }

            // Get or create visitor
            var visitor = await GetOrCreateVisitor(request.SiteId, request.VisitorId, request);

            // Get or create session
            var session = await GetOrCreateSession(request.SiteId, request.VisitorId, request.SessionId, request.Url);

            // Record pageview
            var pageview = new PageView
            {
                SessionId = session.SessionId,
                VisitorId = visitor.VisitorId,
                SiteId = request.SiteId,
                Url = request.Url,
                Path = new Uri(request.Url).AbsolutePath,
                Title = request.Title,
                Referrer = request.Referrer,
                ScreenWidth = request.ScreenWidth,
                ScreenHeight = request.ScreenHeight,
                Timestamp = DateTime.UtcNow
            };

            await _pageviewsCollection.InsertOneAsync(pageview);

            // Update session
            session.Pageviews++;
            session.EndTime = DateTime.UtcNow;
            session.Duration = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;
            await _sessionsCollection.ReplaceOneAsync(s => s.SessionId == session.SessionId, session);

            // Update visitor
            visitor.TotalPageviews++;
            visitor.LastSeen = DateTime.UtcNow;
            await _visitorsCollection.ReplaceOneAsync(v => v.VisitorId == visitor.VisitorId, visitor);

            return Ok(new { success = true, visitorId = visitor.VisitorId, sessionId = session.SessionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking pageview");
            return StatusCode(500, new { success = false, error = "Erro ao registrar pageview" });
        }
    }

    // POST /api/analytics/track/event
    [HttpPost("track/event")]
    [AllowAnonymous]
    public async Task<ActionResult> TrackEvent([FromBody] TrackEventRequest request)
    {
        try
        {
            var analyticsEvent = new AnalyticsEvent
            {
                SessionId = request.SessionId,
                VisitorId = request.VisitorId,
                SiteId = request.SiteId,
                EventName = request.EventName,
                EventCategory = request.EventCategory,
                EventLabel = request.EventLabel,
                EventValue = request.EventValue,
                Url = request.Url,
                Metadata = request.Metadata,
                Timestamp = DateTime.UtcNow
            };

            await _eventsCollection.InsertOneAsync(analyticsEvent);

            // Update session events count
            var session = await _sessionsCollection.Find(s => s.SessionId == request.SessionId).FirstOrDefaultAsync();
            if (session != null)
            {
                session.Events++;
                await _sessionsCollection.ReplaceOneAsync(s => s.SessionId == session.SessionId, session);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking event");
            return StatusCode(500, new { success = false, error = "Erro ao registrar evento" });
        }
    }

    // GET /api/analytics/dashboard/{siteId}
    [HttpGet("dashboard/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetDashboard(string siteId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var filterBuilder = Builders<PageView>.Filter;
            var filter = filterBuilder.Eq(p => p.SiteId, siteId) &
                        filterBuilder.Gte(p => p.Timestamp, startDate) &
                        filterBuilder.Lte(p => p.Timestamp, endDate);

            var totalPageviews = await _pageviewsCollection.CountDocumentsAsync(filter);
            var uniqueVisitors = await _visitorsCollection.CountDocumentsAsync(
                Builders<Visitor>.Filter.Eq(v => v.SiteId, siteId) &
                Builders<Visitor>.Filter.Gte(v => v.LastSeen, startDate));

            var totalSessions = await _sessionsCollection.CountDocumentsAsync(
                Builders<Session>.Filter.Eq(s => s.SiteId, siteId) &
                Builders<Session>.Filter.Gte(s => s.StartTime, startDate));

            var avgSessionDuration = await _sessionsCollection.Aggregate()
                .Match(Builders<Session>.Filter.Eq(s => s.SiteId, siteId) &
                       Builders<Session>.Filter.Gte(s => s.StartTime, startDate))
                .Group(new MongoDB.Bson.BsonDocument
                {
                    { "_id", MongoDB.Bson.BsonNull.Value },
                    { "avgDuration", new MongoDB.Bson.BsonDocument("$avg", "$duration") }
                })
                .FirstOrDefaultAsync();

            var bounceRate = await CalculateBounceRate(siteId, startDate.Value, endDate.Value);

            // Top pages
            var topPages = await _pageviewsCollection.Aggregate()
                .Match(filter)
                .Group(new MongoDB.Bson.BsonDocument
                {
                    { "_id", "$path" },
                    { "views", new MongoDB.Bson.BsonDocument("$sum", 1) },
                    { "title", new MongoDB.Bson.BsonDocument("$first", "$title") }
                })
                .SortByDescending(x => x["views"])
                .Limit(10)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    totalPageviews,
                    uniqueVisitors,
                    totalSessions,
                    avgSessionDuration = avgSessionDuration?["avgDuration"].AsDouble ?? 0,
                    bounceRate,
                    topPages = topPages.Select(p => new
                    {
                        path = p["_id"].AsString,
                        title = p["title"].AsString,
                        views = p["views"].AsInt32
                    })
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return StatusCode(500, new { success = false, error = "Erro ao buscar dados" });
        }
    }

    // GET /api/analytics/realtime/{siteId}
    [HttpGet("realtime/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetRealtime(string siteId)
    {
        try
        {
            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);

            var activeVisitors = await _pageviewsCollection.CountDocumentsAsync(
                Builders<PageView>.Filter.Eq(p => p.SiteId, siteId) &
                Builders<PageView>.Filter.Gte(p => p.Timestamp, fiveMinutesAgo));

            var recentPages = await _pageviewsCollection.Find(
                Builders<PageView>.Filter.Eq(p => p.SiteId, siteId) &
                Builders<PageView>.Filter.Gte(p => p.Timestamp, fiveMinutesAgo))
                .SortByDescending(p => p.Timestamp)
                .Limit(10)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    activeVisitors,
                    recentPages = recentPages.Select(p => new
                    {
                        path = p.Path,
                        title = p.Title,
                        timestamp = p.Timestamp
                    })
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting realtime data");
            return StatusCode(500, new { success = false, error = "Erro ao buscar dados em tempo real" });
        }
    }

    private async Task<Visitor> GetOrCreateVisitor(string siteId, string visitorId, TrackPageviewRequest request)
    {
        var visitor = await _visitorsCollection.Find(v => v.VisitorId == visitorId).FirstOrDefaultAsync();
        
        if (visitor == null)
        {
            visitor = new Visitor
            {
                VisitorId = visitorId,
                SiteId = siteId,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                Browser = request.Browser,
                OS = request.OS,
                Device = request.Device,
                Referrer = request.Referrer,
                Country = request.Country,
                City = request.City,
                UtmSource = request.UtmSource,
                UtmMedium = request.UtmMedium,
                UtmCampaign = request.UtmCampaign
            };
            await _visitorsCollection.InsertOneAsync(visitor);
        }

        return visitor;
    }

    private async Task<Session> GetOrCreateSession(string siteId, string visitorId, string sessionId, string url)
    {
        var session = await _sessionsCollection.Find(s => s.SessionId == sessionId).FirstOrDefaultAsync();
        
        if (session == null)
        {
            session = new Session
            {
                SessionId = sessionId,
                VisitorId = visitorId,
                SiteId = siteId,
                StartTime = DateTime.UtcNow,
                LandingPage = url
            };
            await _sessionsCollection.InsertOneAsync(session);

            // Update visitor sessions count
            var visitor = await _visitorsCollection.Find(v => v.VisitorId == visitorId).FirstOrDefaultAsync();
            if (visitor != null)
            {
                visitor.TotalSessions++;
                await _visitorsCollection.ReplaceOneAsync(v => v.VisitorId == visitorId, visitor);
            }
        }

        return session;
    }

    private async Task<double> CalculateBounceRate(string siteId, DateTime startDate, DateTime endDate)
    {
        var sessions = await _sessionsCollection.Find(
            Builders<Session>.Filter.Eq(s => s.SiteId, siteId) &
            Builders<Session>.Filter.Gte(s => s.StartTime, startDate) &
            Builders<Session>.Filter.Lte(s => s.StartTime, endDate))
            .ToListAsync();

        if (!sessions.Any()) return 0;

        var bouncedSessions = sessions.Count(s => s.Pageviews <= 1);
        return (double)bouncedSessions / sessions.Count * 100;
    }
}

// Request DTOs
public class TrackPageviewRequest
{
    public string SiteId { get; set; } = string.Empty;
    public string VisitorId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Referrer { get; set; }
    public string? Browser { get; set; }
    public string? OS { get; set; }
    public string? Device { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
}

public class TrackEventRequest
{
    public string SiteId { get; set; } = string.Empty;
    public string VisitorId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? EventCategory { get; set; }
    public string? EventLabel { get; set; }
    public decimal? EventValue { get; set; }
    public string? Url { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
