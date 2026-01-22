using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IMongoCollection<ChatMessage> _messagesCollection;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IMongoCollection<ChatMessage> messagesCollection,
        ILogger<ChatHub> logger)
    {
        _messagesCollection = messagesCollection;
        _logger = logger;
    }

    public async Task JoinSite(string siteId, string visitorId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"site_{siteId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"visitor_{visitorId}");
        
        _logger.LogInformation("Visitor {VisitorId} joined site {SiteId}", visitorId, siteId);
    }

    public async Task SendMessage(string siteId, string visitorId, string message, string? visitorName = null, string? visitorEmail = null)
    {
        try
        {
            var chatMessage = new ChatMessage
            {
                SiteId = siteId,
                VisitorId = visitorId,
                VisitorName = visitorName,
                VisitorEmail = visitorEmail,
                Message = message,
                Sender = "visitor",
                Timestamp = DateTime.UtcNow
            };

            await _messagesCollection.InsertOneAsync(chatMessage);

            // Send to all agents monitoring this site
            await Clients.Group($"site_{siteId}").SendAsync("ReceiveMessage", new
            {
                chatMessage.ChatId,
                chatMessage.VisitorId,
                chatMessage.VisitorName,
                chatMessage.Message,
                chatMessage.Sender,
                chatMessage.Timestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
        }
    }

    public async Task SendAgentMessage(string chatId, string visitorId, string agentId, string message)
    {
        try
        {
            var chatMessage = new ChatMessage
            {
                ChatId = chatId,
                VisitorId = visitorId,
                AgentId = agentId,
                Message = message,
                Sender = "agent",
                Timestamp = DateTime.UtcNow
            };

            await _messagesCollection.InsertOneAsync(chatMessage);

            // Send to specific visitor
            await Clients.Group($"visitor_{visitorId}").SendAsync("ReceiveMessage", new
            {
                chatMessage.ChatId,
                chatMessage.Message,
                chatMessage.Sender,
                chatMessage.Timestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending agent message");
        }
    }

    public async Task MarkAsRead(string chatId)
    {
        try
        {
            var filter = Builders<ChatMessage>.Filter.Eq(m => m.ChatId, chatId);
            var update = Builders<ChatMessage>.Update.Set(m => m.Read, true);
            
            await _messagesCollection.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
