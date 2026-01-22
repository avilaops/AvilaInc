using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Infrastructure.Services;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/gmail")]
[Authorize]
public class GmailController : ControllerBase
{
    private readonly IGmailService _gmailService;
    private readonly IMongoCollection<Email> _emailCollection;
    private readonly ILogger<GmailController> _logger;
    private readonly IConfiguration _configuration;

    public GmailController(
        IGmailService gmailService,
        IMongoCollection<Email> emailCollection,
        ILogger<GmailController> logger,
        IConfiguration configuration)
    {
        _gmailService = gmailService;
        _emailCollection = emailCollection;
        _logger = logger;
        _configuration = configuration;
    }

    #region Authentication

    /// <summary>
    /// Obter URL de autorização OAuth2 para uma conta
    /// </summary>
    [HttpGet("auth/{account}")]
    public async Task<IActionResult> GetAuthUrl(string account)
    {
        try
        {
            var url = await _gmailService.GetAuthorizationUrlAsync(account);

            return Ok(new
            {
                success = true,
                account,
                authUrl = url,
                message = "Abra esta URL no navegador para autorizar a conta"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar URL de autenticação");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Callback OAuth2 (AllowAnonymous)
    /// </summary>
    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> OAuthCallback([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            var account = state; // state contém o account
            var credential = await _gmailService.AuthorizeAsync(account, code);

            return Ok(new
            {
                success = true,
                account,
                message = "Conta autorizada com sucesso! Você pode fechar esta janela."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no callback OAuth2");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Messages

    /// <summary>
    /// Listar emails de uma conta
    /// </summary>
    [HttpGet("{account}/messages")]
    public async Task<IActionResult> ListMessages(
        string account,
        [FromQuery] string? query = null,
        [FromQuery] int maxResults = 100)
    {
        try
        {
            var response = await _gmailService.ListMessagesAsync(account, query, maxResults);

            return Ok(new
            {
                success = true,
                account,
                count = response.Messages?.Count ?? 0,
                resultSizeEstimate = response.ResultSizeEstimate,
                nextPageToken = response.NextPageToken,
                messages = response.Messages?.Select(m => new
                {
                    id = m.Id,
                    threadId = m.ThreadId
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar emails da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Obter email completo por ID
    /// </summary>
    [HttpGet("{account}/messages/{messageId}")]
    public async Task<IActionResult> GetMessage(string account, string messageId)
    {
        try
        {
            var message = await _gmailService.GetMessageAsync(account, messageId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = message.Id,
                    threadId = message.ThreadId,
                    labelIds = message.LabelIds,
                    snippet = message.Snippet,
                    internalDate = message.InternalDate,
                    payload = message.Payload,
                    sizeEstimate = message.SizeEstimate
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter email {MessageId}", messageId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Enviar email
    /// </summary>
    [HttpPost("{account}/messages/send")]
    public async Task<IActionResult> SendMessage(string account, [FromBody] SendEmailRequest request)
    {
        try
        {
            var message = await _gmailService.SendMessageAsync(
                account,
                request.To,
                request.Subject,
                request.Body,
                request.IsHtml
            );

            return Ok(new
            {
                success = true,
                messageId = message.Id,
                threadId = message.ThreadId,
                message = "Email enviado com sucesso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Deletar email
    /// </summary>
    [HttpDelete("{account}/messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string account, string messageId)
    {
        try
        {
            await _gmailService.DeleteMessageAsync(account, messageId);

            return Ok(new
            {
                success = true,
                message = "Email deletado com sucesso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar email {MessageId}", messageId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Modificar labels de um email (marcar como lido, starred, etc)
    /// </summary>
    [HttpPut("{account}/messages/{messageId}/labels")]
    public async Task<IActionResult> ModifyMessage(
        string account,
        string messageId,
        [FromBody] ModifyLabelsRequest request)
    {
        try
        {
            await _gmailService.ModifyMessageAsync(account, messageId, request.AddLabels, request.RemoveLabels);

            return Ok(new
            {
                success = true,
                message = "Labels modificadas com sucesso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao modificar email {MessageId}", messageId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Threads

    /// <summary>
    /// Listar threads (conversas)
    /// </summary>
    [HttpGet("{account}/threads")]
    public async Task<IActionResult> ListThreads(
        string account,
        [FromQuery] string? query = null,
        [FromQuery] int maxResults = 100)
    {
        try
        {
            var response = await _gmailService.ListThreadsAsync(account, query, maxResults);

            return Ok(new
            {
                success = true,
                account,
                count = response.Threads?.Count ?? 0,
                resultSizeEstimate = response.ResultSizeEstimate,
                nextPageToken = response.NextPageToken,
                threads = response.Threads?.Select(t => new
                {
                    id = t.Id,
                    snippet = t.Snippet
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar threads da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Obter thread completa
    /// </summary>
    [HttpGet("{account}/threads/{threadId}")]
    public async Task<IActionResult> GetThread(string account, string threadId)
    {
        try
        {
            var thread = await _gmailService.GetThreadAsync(account, threadId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = thread.Id,
                    snippet = thread.Snippet,
                    historyId = thread.HistoryId,
                    messageCount = thread.Messages?.Count ?? 0,
                    messages = thread.Messages?.Select(m => new
                    {
                        id = m.Id,
                        snippet = m.Snippet,
                        internalDate = m.InternalDate,
                        labelIds = m.LabelIds
                    })
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter thread {ThreadId}", threadId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Labels

    /// <summary>
    /// Listar labels (categorias) de uma conta
    /// </summary>
    [HttpGet("{account}/labels")]
    public async Task<IActionResult> ListLabels(string account)
    {
        try
        {
            var response = await _gmailService.ListLabelsAsync(account);

            return Ok(new
            {
                success = true,
                account,
                count = response.Labels?.Count ?? 0,
                labels = response.Labels?.Select(l => new
                {
                    id = l.Id,
                    name = l.Name,
                    type = l.Type,
                    messageListVisibility = l.MessageListVisibility,
                    labelListVisibility = l.LabelListVisibility,
                    messagesTotal = l.MessagesTotal,
                    messagesUnread = l.MessagesUnread
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar labels da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar label customizada
    /// </summary>
    [HttpPost("{account}/labels")]
    public async Task<IActionResult> CreateLabel(string account, [FromBody] CreateLabelRequest request)
    {
        try
        {
            var label = await _gmailService.CreateLabelAsync(account, request.Name, request.Color);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = label.Id,
                    name = label.Name,
                    color = label.Color
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar label na conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Search & Sync

    /// <summary>
    /// Buscar emails com query (ex: "from:nicolas@avila.inc subject:importante")
    /// </summary>
    [HttpGet("{account}/search")]
    public async Task<IActionResult> SearchMessages(
        string account,
        [FromQuery] string query,
        [FromQuery] int maxResults = 50)
    {
        try
        {
            var messages = await _gmailService.SearchMessagesAsync(account, query, maxResults);

            return Ok(new
            {
                success = true,
                account,
                query,
                count = messages.Count,
                messages = messages.Select(m => new
                {
                    id = m.Id,
                    threadId = m.ThreadId,
                    snippet = m.Snippet,
                    labelIds = m.LabelIds,
                    internalDate = m.InternalDate
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar emails na conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Sincronizar conta com MongoDB (salvar emails localmente)
    /// </summary>
    [HttpPost("{account}/sync")]
    public async Task<IActionResult> SyncAccount(string account, [FromQuery] DateTime? since = null)
    {
        try
        {
            var syncedCount = await _gmailService.SyncAccountToMongoDbAsync(account, since);

            return Ok(new
            {
                success = true,
                account,
                syncedCount,
                message = $"{syncedCount} emails novos sincronizados"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Listar emails do MongoDB (já sincronizados)
    /// </summary>
    [HttpGet("{account}/local")]
    public async Task<IActionResult> GetLocalEmails(
        string account,
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 50,
        [FromQuery] bool? unreadOnly = null)
    {
        try
        {
            var filterBuilder = Builders<Email>.Filter;
            var filter = filterBuilder.Eq(e => e.Account, account);

            if (unreadOnly == true)
            {
                filter &= filterBuilder.Eq(e => e.IsRead, false);
            }

            var emails = await _emailCollection
                .Find(filter)
                .SortByDescending(e => e.Date)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            var totalCount = await _emailCollection.CountDocumentsAsync(filter);
            var unreadCount = await _emailCollection.CountDocumentsAsync(
                filterBuilder.Eq(e => e.Account, account) & filterBuilder.Eq(e => e.IsRead, false));

            return Ok(new
            {
                success = true,
                account,
                count = emails.Count,
                totalCount,
                unreadCount,
                skip,
                limit,
                data = emails.Select(e => new
                {
                    id = e.Id,
                    messageId = e.MessageId,
                    threadId = e.ThreadId,
                    from = e.From,
                    to = e.To,
                    subject = e.Subject,
                    snippet = e.Snippet,
                    labels = e.Labels,
                    isRead = e.IsRead,
                    isStarred = e.IsStarred,
                    hasAttachments = e.HasAttachments,
                    date = e.Date,
                    syncedAt = e.SyncedAt
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar emails locais da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Sincronizar todas as contas configuradas
    /// </summary>
    [HttpPost("sync-all")]
    public async Task<IActionResult> SyncAllAccounts()
    {
        try
        {
            var accounts = _configuration.GetSection("Integrations:Gmail:Accounts").Get<List<string>>() ?? new List<string>();

            var results = new List<object>();

            foreach (var account in accounts)
            {
                try
                {
                    var syncedCount = await _gmailService.SyncAccountToMongoDbAsync(account);
                    results.Add(new
                    {
                        account,
                        success = true,
                        syncedCount
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao sincronizar conta {Account}", account);
                    results.Add(new
                    {
                        account,
                        success = false,
                        error = ex.Message
                    });
                }
            }

            return Ok(new
            {
                success = true,
                totalAccounts = accounts.Count,
                results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar todas as contas");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Attachments

    /// <summary>
    /// Baixar attachment de um email
    /// </summary>
    [HttpGet("{account}/messages/{messageId}/attachments/{attachmentId}")]
    public async Task<IActionResult> GetAttachment(string account, string messageId, string attachmentId)
    {
        try
        {
            var attachment = await _gmailService.GetAttachmentAsync(account, messageId, attachmentId);

            // Decode base64
            var data = attachment.Data.Replace('-', '+').Replace('_', '/');
            var bytes = Convert.FromBase64String(data);

            return File(bytes, "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter attachment {AttachmentId}", attachmentId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Stats

    /// <summary>
    /// Estatísticas de emails por conta
    /// </summary>
    [HttpGet("{account}/stats")]
    public async Task<IActionResult> GetStats(string account)
    {
        try
        {
            var filter = Builders<Email>.Filter.Eq(e => e.Account, account);

            var totalCount = await _emailCollection.CountDocumentsAsync(filter);
            var unreadCount = await _emailCollection.CountDocumentsAsync(
                filter & Builders<Email>.Filter.Eq(e => e.IsRead, false));
            var starredCount = await _emailCollection.CountDocumentsAsync(
                filter & Builders<Email>.Filter.Eq(e => e.IsStarred, true));
            var withAttachmentsCount = await _emailCollection.CountDocumentsAsync(
                filter & Builders<Email>.Filter.Eq(e => e.HasAttachments, true));

            var lastSyncedEmail = await _emailCollection
                .Find(filter)
                .SortByDescending(e => e.SyncedAt)
                .Limit(1)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                success = true,
                account,
                stats = new
                {
                    totalEmails = totalCount,
                    unreadEmails = unreadCount,
                    starredEmails = starredCount,
                    emailsWithAttachments = withAttachmentsCount,
                    lastSync = lastSyncedEmail?.SyncedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter stats da conta {Account}", account);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public record SendEmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false
);

public record ModifyLabelsRequest(
    List<string>? AddLabels = null,
    List<string>? RemoveLabels = null
);

public record CreateLabelRequest(
    string Name,
    string Color = "#cccccc"
);

#endregion
