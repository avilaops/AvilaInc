using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IMongoCollection<ChatMessage> _messagesCollection;
    private readonly IMongoCollection<Site> _sitesCollection;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IMongoCollection<ChatMessage> messagesCollection,
        IMongoCollection<Site> sitesCollection,
        ILogger<ChatController> logger)
    {
        _messagesCollection = messagesCollection;
        _sitesCollection = sitesCollection;
        _logger = logger;
    }

    // GET /api/chat/messages/{siteId}
    [HttpGet("messages/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetMessages(string siteId, [FromQuery] string? visitorId = null)
    {
        try
        {
            var filterBuilder = Builders<ChatMessage>.Filter;
            var filter = filterBuilder.Eq(m => m.SiteId, siteId);

            if (!string.IsNullOrEmpty(visitorId))
            {
                filter &= filterBuilder.Eq(m => m.VisitorId, visitorId);
            }

            var messages = await _messagesCollection
                .Find(filter)
                .SortBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(new { success = true, data = messages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages");
            return StatusCode(500, new { success = false, error = "Erro ao buscar mensagens" });
        }
    }

    // GET /api/chat/conversations/{siteId}
    [HttpGet("conversations/{siteId}")]
    [Authorize]
    public async Task<ActionResult> GetConversations(string siteId)
    {
        try
        {
            var conversations = await _messagesCollection.Aggregate()
                .Match(Builders<ChatMessage>.Filter.Eq(m => m.SiteId, siteId))
                .Group(new MongoDB.Bson.BsonDocument
                {
                    { "_id", "$visitorId" },
                    { "visitorName", new MongoDB.Bson.BsonDocument("$first", "$visitorName") },
                    { "visitorEmail", new MongoDB.Bson.BsonDocument("$first", "$visitorEmail") },
                    { "lastMessage", new MongoDB.Bson.BsonDocument("$last", "$message") },
                    { "lastTimestamp", new MongoDB.Bson.BsonDocument("$last", "$timestamp") },
                    { "messageCount", new MongoDB.Bson.BsonDocument("$sum", 1) },
                    { "unreadCount", new MongoDB.Bson.BsonDocument("$sum", new MongoDB.Bson.BsonDocument("$cond", new MongoDB.Bson.BsonArray { "$read", 0, 1 })) }
                })
                .SortByDescending(x => x["lastTimestamp"])
                .ToListAsync();

            var result = conversations.Select(c => new
            {
                visitorId = c["_id"].AsString,
                visitorName = c["visitorName"].AsString,
                visitorEmail = c.Contains("visitorEmail") ? c["visitorEmail"].AsString : null,
                lastMessage = c["lastMessage"].AsString,
                lastTimestamp = c["lastTimestamp"].ToUniversalTime(),
                messageCount = c["messageCount"].AsInt32,
                unreadCount = c["unreadCount"].AsInt32
            });

            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversations");
            return StatusCode(500, new { success = false, error = "Erro ao buscar conversas" });
        }
    }

    // GET /api/chat/widget/{siteId}
    [HttpGet("widget/{siteId}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetWidgetConfig(string siteId)
    {
        try
        {
            var site = await _sitesCollection
                .Find(s => s.SiteId == siteId && s.Active && s.ChatEnabled)
                .FirstOrDefaultAsync();

            if (site == null)
            {
                return NotFound(new { success = false, error = "Chat não habilitado para este site" });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    enabled = true,
                    config = site.ChatWidget
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting widget config");
            return StatusCode(500, new { success = false, error = "Erro ao buscar configuração" });
        }
    }
}
