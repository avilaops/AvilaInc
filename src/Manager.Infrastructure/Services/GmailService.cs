using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text;

namespace Manager.Infrastructure.Services;

public interface IGmailService
{
    // Authentication
    Task<string> GetAuthorizationUrlAsync(string account);
    Task<UserCredential> AuthorizeAsync(string account, string code);

    // Messages
    Task<ListMessagesResponse> ListMessagesAsync(string account, string? query = null, int maxResults = 100);
    Task<Message> GetMessageAsync(string account, string messageId);
    Task<Message> SendMessageAsync(string account, string to, string subject, string body, bool isHtml = false);
    Task<string> SendRawMessageAsync(string account, string rawMessage);
    Task DeleteMessageAsync(string account, string messageId);
    Task ModifyMessageAsync(string account, string messageId, List<string>? addLabels = null, List<string>? removeLabels = null);

    // Threads
    Task<ListThreadsResponse> ListThreadsAsync(string account, string? query = null, int maxResults = 100);
    Task<Google.Apis.Gmail.v1.Data.Thread> GetThreadAsync(string account, string threadId);

    // Labels
    Task<ListLabelsResponse> ListLabelsAsync(string account);
    Task<Label> CreateLabelAsync(string account, string name, string color);
    Task<Label> GetLabelAsync(string account, string labelId);

    // Search & Sync
    Task<List<Message>> SearchMessagesAsync(string account, string query, int maxResults = 50);
    Task<int> SyncAccountToMongoDbAsync(string account, DateTime? since = null);

    // Attachments
    Task<MessagePartBody> GetAttachmentAsync(string account, string messageId, string attachmentId);
}

public class GmailService : IGmailService
{
    private readonly ILogger<GmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<Manager.Core.Entities.Email> _emailCollection;
    private readonly Dictionary<string, GmailServiceClient> _gmailClients = new();

    private class GmailServiceClient
    {
        public string Account { get; set; } = string.Empty;
        public Google.Apis.Gmail.v1.GmailService Service { get; set; } = null!;
        public UserCredential? Credential { get; set; }
    }

    public GmailService(
        IConfiguration configuration,
        ILogger<GmailService> logger,
        IMongoCollection<Manager.Core.Entities.Email> emailCollection)
    {
        _configuration = configuration;
        _logger = logger;
        _emailCollection = emailCollection;
    }

    private async Task<Google.Apis.Gmail.v1.GmailService> GetGmailServiceAsync(string account)
    {
        if (_gmailClients.TryGetValue(account, out var client))
        {
            return client.Service;
        }

        var clientId = _configuration["Integrations:Gmail:ClientId"];
        var clientSecret = _configuration["Integrations:Gmail:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Gmail credentials não configuradas");
        }

        var secrets = new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            new[] { Google.Apis.Gmail.v1.GmailService.Scope.GmailModify },
            account,
            CancellationToken.None);

        var service = new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Avila Manager"
        });

        _gmailClients[account] = new GmailServiceClient
        {
            Account = account,
            Service = service,
            Credential = credential
        };

        _logger.LogInformation("Gmail service inicializado para conta {Account}", account);

        return service;
    }

    #region Authentication

    public async Task<string> GetAuthorizationUrlAsync(string account)
    {
        var clientId = _configuration["Integrations:Gmail:ClientId"];
        var redirectUri = _configuration["Integrations:Gmail:RedirectUri"] ?? "http://localhost:5056/api/gmail/callback";

        var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                  $"client_id={clientId}&" +
                  $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                  $"response_type=code&" +
                  $"scope={Uri.EscapeDataString("https://www.googleapis.com/auth/gmail.modify")}&" +
                  $"access_type=offline&" +
                  $"state={account}";

        return await Task.FromResult(url);
    }

    public async Task<UserCredential> AuthorizeAsync(string account, string code)
    {
        var clientId = _configuration["Integrations:Gmail:ClientId"];
        var clientSecret = _configuration["Integrations:Gmail:ClientSecret"];
        var redirectUri = _configuration["Integrations:Gmail:RedirectUri"] ?? "http://localhost:5056/api/gmail/callback";

        var secrets = new ClientSecrets
        {
            ClientId = clientId!,
            ClientSecret = clientSecret!
        };

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            new[] { Google.Apis.Gmail.v1.GmailService.Scope.GmailModify },
            account,
            CancellationToken.None);

        _logger.LogInformation("Conta {Account} autorizada com sucesso", account);

        return credential;
    }

    #endregion

    #region Messages

    public async Task<ListMessagesResponse> ListMessagesAsync(string account, string? query = null, int maxResults = 100)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Messages.List("me");
            request.MaxResults = maxResults;
            if (!string.IsNullOrEmpty(query))
            {
                request.Q = query;
            }

            var response = await request.ExecuteAsync();
            _logger.LogInformation("Listados {Count} emails da conta {Account}", response.Messages?.Count ?? 0, account);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar emails da conta {Account}", account);
            throw;
        }
    }

    public async Task<Message> GetMessageAsync(string account, string messageId)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Messages.Get("me", messageId);
            request.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;

            var message = await request.ExecuteAsync();
            _logger.LogInformation("Email {MessageId} obtido da conta {Account}", messageId, account);

            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter email {MessageId} da conta {Account}", messageId, account);
            throw;
        }
    }

    public async Task<Message> SendMessageAsync(string account, string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var contentType = isHtml ? "text/html" : "text/plain";
            var emailContent = $"To: {to}\r\n" +
                              $"Subject: {subject}\r\n" +
                              $"Content-Type: {contentType}; charset=utf-8\r\n\r\n" +
                              $"{body}";

            var encodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailContent))
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            var message = new Message { Raw = encodedEmail };

            var request = service.Users.Messages.Send(message, "me");
            var sentMessage = await request.ExecuteAsync();

            _logger.LogInformation("Email enviado para {To} da conta {Account}", to, account);

            return sentMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email da conta {Account}", account);
            throw;
        }
    }

    public async Task<string> SendRawMessageAsync(string account, string rawMessage)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var encodedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawMessage))
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");

            var message = new Message { Raw = encodedEmail };

            var request = service.Users.Messages.Send(message, "me");
            var sentMessage = await request.ExecuteAsync();

            _logger.LogInformation("Raw email enviado da conta {Account}", account);

            return sentMessage.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar raw email da conta {Account}", account);
            throw;
        }
    }

    public async Task DeleteMessageAsync(string account, string messageId)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Messages.Delete("me", messageId);
            await request.ExecuteAsync();

            _logger.LogInformation("Email {MessageId} deletado da conta {Account}", messageId, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar email {MessageId} da conta {Account}", messageId, account);
            throw;
        }
    }

    public async Task ModifyMessageAsync(string account, string messageId, List<string>? addLabels = null, List<string>? removeLabels = null)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var modifyRequest = new ModifyMessageRequest
            {
                AddLabelIds = addLabels,
                RemoveLabelIds = removeLabels
            };

            var request = service.Users.Messages.Modify(modifyRequest, "me", messageId);
            await request.ExecuteAsync();

            _logger.LogInformation("Email {MessageId} modificado na conta {Account}", messageId, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao modificar email {MessageId} da conta {Account}", messageId, account);
            throw;
        }
    }

    #endregion

    #region Threads

    public async Task<ListThreadsResponse> ListThreadsAsync(string account, string? query = null, int maxResults = 100)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Threads.List("me");
            request.MaxResults = maxResults;
            if (!string.IsNullOrEmpty(query))
            {
                request.Q = query;
            }

            var response = await request.ExecuteAsync();
            _logger.LogInformation("Listadas {Count} threads da conta {Account}", response.Threads?.Count ?? 0, account);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar threads da conta {Account}", account);
            throw;
        }
    }

    public async Task<Google.Apis.Gmail.v1.Data.Thread> GetThreadAsync(string account, string threadId)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Threads.Get("me", threadId);
            var thread = await request.ExecuteAsync();

            _logger.LogInformation("Thread {ThreadId} obtida da conta {Account}", threadId, account);

            return thread;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter thread {ThreadId} da conta {Account}", threadId, account);
            throw;
        }
    }

    #endregion

    #region Labels

    public async Task<ListLabelsResponse> ListLabelsAsync(string account)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Labels.List("me");
            var response = await request.ExecuteAsync();

            _logger.LogInformation("Listadas {Count} labels da conta {Account}", response.Labels?.Count ?? 0, account);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar labels da conta {Account}", account);
            throw;
        }
    }

    public async Task<Label> CreateLabelAsync(string account, string name, string color)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var label = new Label
            {
                Name = name,
                LabelListVisibility = "labelShow",
                MessageListVisibility = "show",
                Color = new LabelColor { BackgroundColor = color }
            };

            var request = service.Users.Labels.Create(label, "me");
            var createdLabel = await request.ExecuteAsync();

            _logger.LogInformation("Label {LabelName} criada na conta {Account}", name, account);

            return createdLabel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar label {LabelName} na conta {Account}", name, account);
            throw;
        }
    }

    public async Task<Label> GetLabelAsync(string account, string labelId)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Labels.Get("me", labelId);
            var label = await request.ExecuteAsync();

            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter label {LabelId} da conta {Account}", labelId, account);
            throw;
        }
    }

    #endregion

    #region Search & Sync

    public async Task<List<Message>> SearchMessagesAsync(string account, string query, int maxResults = 50)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var listRequest = service.Users.Messages.List("me");
            listRequest.Q = query;
            listRequest.MaxResults = maxResults;

            var response = await listRequest.ExecuteAsync();

            var messages = new List<Message>();

            if (response.Messages != null)
            {
                foreach (var messageRef in response.Messages)
                {
                    var message = await GetMessageAsync(account, messageRef.Id);
                    messages.Add(message);
                }
            }

            _logger.LogInformation("Busca '{Query}' retornou {Count} emails da conta {Account}", query, messages.Count, account);

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar emails na conta {Account}", account);
            throw;
        }
    }

    public async Task<int> SyncAccountToMongoDbAsync(string account, DateTime? since = null)
    {
        try
        {
            var query = since.HasValue 
                ? $"after:{since.Value:yyyy/MM/dd}" 
                : "in:inbox OR in:sent";

            var listResponse = await ListMessagesAsync(account, query, 500);

            var syncedCount = 0;

            if (listResponse.Messages != null)
            {
                foreach (var messageRef in listResponse.Messages)
                {
                    // Verificar se já existe
                    var existing = await _emailCollection
                        .Find(e => e.Account == account && e.MessageId == messageRef.Id)
                        .FirstOrDefaultAsync();

                    if (existing != null)
                        continue;

                    var message = await GetMessageAsync(account, messageRef.Id);

                    var emailEntity = new Manager.Core.Entities.Email
                    {
                        Account = account,
                        MessageId = message.Id,
                        ThreadId = message.ThreadId,
                        From = GetHeaderValue(message, "From"),
                        To = GetHeaderValue(message, "To").Split(',').Select(s => s.Trim()).ToList(),
                        Subject = GetHeaderValue(message, "Subject"),
                        Snippet = message.Snippet,
                        Body = GetMessageBody(message),
                        Labels = message.LabelIds?.ToList() ?? new List<string>(),
                        IsRead = !message.LabelIds?.Contains("UNREAD") ?? true,
                        Date = DateTimeOffset.FromUnixTimeMilliseconds(message.InternalDate ?? 0).DateTime,
                        HasAttachments = message.Payload?.Parts?.Any(p => !string.IsNullOrEmpty(p.Filename)) ?? false
                    };

                    await _emailCollection.InsertOneAsync(emailEntity);
                    syncedCount++;
                }
            }

            _logger.LogInformation("Sync da conta {Account}: {Count} emails novos", account, syncedCount);

            return syncedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar conta {Account}", account);
            throw;
        }
    }

    #endregion

    #region Attachments

    public async Task<MessagePartBody> GetAttachmentAsync(string account, string messageId, string attachmentId)
    {
        try
        {
            var service = await GetGmailServiceAsync(account);

            var request = service.Users.Messages.Attachments.Get("me", messageId, attachmentId);
            var attachment = await request.ExecuteAsync();

            _logger.LogInformation("Attachment {AttachmentId} obtido do email {MessageId}", attachmentId, messageId);

            return attachment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter attachment {AttachmentId}", attachmentId);
            throw;
        }
    }

    #endregion

    #region Helper Methods

    private string GetHeaderValue(Message message, string headerName)
    {
        var header = message.Payload?.Headers?.FirstOrDefault(h => 
            h.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));
        return header?.Value ?? string.Empty;
    }

    private string GetMessageBody(Message message)
    {
        if (message.Payload == null)
            return string.Empty;

        // Try to get plain text body
        var textPart = FindPartByMimeType(message.Payload, "text/plain");
        if (textPart?.Body?.Data != null)
        {
            return DecodeBase64(textPart.Body.Data);
        }

        // Fallback to HTML
        var htmlPart = FindPartByMimeType(message.Payload, "text/html");
        if (htmlPart?.Body?.Data != null)
        {
            return DecodeBase64(htmlPart.Body.Data);
        }

        // Direct body
        if (message.Payload.Body?.Data != null)
        {
            return DecodeBase64(message.Payload.Body.Data);
        }

        return message.Snippet ?? string.Empty;
    }

    private MessagePart? FindPartByMimeType(MessagePart part, string mimeType)
    {
        if (part.MimeType == mimeType)
            return part;

        if (part.Parts != null)
        {
            foreach (var subPart in part.Parts)
            {
                var found = FindPartByMimeType(subPart, mimeType);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private string DecodeBase64(string base64)
    {
        try
        {
            var base64Fixed = base64.Replace('-', '+').Replace('_', '/');
            var bytes = Convert.FromBase64String(base64Fixed);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return base64;
        }
    }

    #endregion
}
