using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;
using System.Linq.Expressions;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/crm")]
[Authorize]
public class CrmController : ControllerBase
{
    private readonly IMongoCollection<Lead> _leadsCollection;
    private readonly IMongoCollection<Contact> _contactsCollection;
    private readonly ILogger<CrmController> _logger;

    public CrmController(
        IMongoCollection<Lead> leadsCollection,
        IMongoCollection<Contact> contactsCollection,
        ILogger<CrmController> logger)
    {
        _leadsCollection = leadsCollection;
        _contactsCollection = contactsCollection;
        _logger = logger;
    }

    #region Leads

    // GET /api/crm/leads
    [HttpGet("leads")]
    public async Task<ActionResult<IEnumerable<Lead>>> GetLeads(
        [FromQuery] string? status = null,
        [FromQuery] string? origem = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var filterBuilder = Builders<Lead>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter &= filterBuilder.Eq(l => l.Status, status);
            }

            if (!string.IsNullOrEmpty(origem))
            {
                filter &= filterBuilder.Eq(l => l.Origem, origem);
            }

            var leads = await _leadsCollection
                .Find(filter)
                .Sort(Builders<Lead>.Sort.Descending(l => l.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            var total = await _leadsCollection.CountDocumentsAsync(filter);

            return Ok(new
            {
                data = leads,
                pagination = new
                {
                    page,
                    pageSize,
                    total,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching leads");
            return StatusCode(500, new { error = "Erro ao buscar leads" });
        }
    }

    // GET /api/crm/leads/{id}
    [HttpGet("leads/{id}")]
    public async Task<ActionResult<Lead>> GetLead(string id)
    {
        try
        {
            var lead = await _leadsCollection
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (lead == null)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            return Ok(lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching lead {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao buscar lead" });
        }
    }

    // POST /api/crm/leads
    [HttpPost("leads")]
    public async Task<ActionResult<Lead>> CreateLead([FromBody] Lead lead)
    {
        try
        {
            lead.CreatedAt = DateTime.UtcNow;
            lead.UpdatedAt = DateTime.UtcNow;

            await _leadsCollection.InsertOneAsync(lead);

            _logger.LogInformation("Lead created: {LeadId}", lead.Id);

            return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            return StatusCode(500, new { error = "Erro ao criar lead" });
        }
    }

    // PUT /api/crm/leads/{id}
    [HttpPut("leads/{id}")]
    public async Task<ActionResult<Lead>> UpdateLead(string id, [FromBody] Lead lead)
    {
        try
        {
            var existingLead = await _leadsCollection
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (existingLead == null)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            lead.Id = id;
            lead.CreatedAt = existingLead.CreatedAt;
            lead.UpdatedAt = DateTime.UtcNow;

            await _leadsCollection.ReplaceOneAsync(l => l.Id == id, lead);

            _logger.LogInformation("Lead updated: {LeadId}", id);

            return Ok(lead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao atualizar lead" });
        }
    }

    // PUT /api/crm/leads/{id}/status
    [HttpPut("leads/{id}/status")]
    public async Task<ActionResult> UpdateLeadStatus(
        string id,
        [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var update = Builders<Lead>.Update
                .Set(l => l.Status, request.Status)
                .Set(l => l.UpdatedAt, DateTime.UtcNow);

            if (request.Status == "ganho")
            {
                update = update.Set(l => l.DataConversao, DateTime.UtcNow);
            }

            var result = await _leadsCollection.UpdateOneAsync(
                l => l.Id == id,
                update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            _logger.LogInformation("Lead status updated: {LeadId} -> {Status}", id, request.Status);

            return Ok(new { message = "Status atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead status {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao atualizar status" });
        }
    }

    // DELETE /api/crm/leads/{id}
    [HttpDelete("leads/{id}")]
    public async Task<ActionResult> DeleteLead(string id)
    {
        try
        {
            var result = await _leadsCollection.DeleteOneAsync(l => l.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            _logger.LogInformation("Lead deleted: {LeadId}", id);

            return Ok(new { message = "Lead removido com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao remover lead" });
        }
    }

    // POST /api/crm/leads/patent/verify
    [HttpPost("leads/patent/verify")]
    public async Task<ActionResult> VerifyPatent([FromBody] VerifyPatentRequest request)
    {
        try
        {
            // TODO: Implementar integração com API de patentes
            // Por enquanto, apenas marca como verificado

            var update = Builders<Lead>.Update
                .Set(l => l.PatentVerified, true)
                .Set(l => l.HasPatent, request.HasPatent)
                .Set(l => l.UpdatedAt, DateTime.UtcNow);

            var result = await _leadsCollection.UpdateOneAsync(
                l => l.Email == request.Email,
                update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            return Ok(new
            {
                message = "Verificação de patente concluída",
                hasPatent = request.HasPatent
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying patent for {Email}", request.Email);
            return StatusCode(500, new { error = "Erro ao verificar patente" });
        }
    }

    // POST /api/crm/leads/{id}/patent/skip
    [HttpPost("leads/{id}/patent/skip")]
    public async Task<ActionResult> SkipPatentVerification(string id)
    {
        try
        {
            var update = Builders<Lead>.Update
                .Set(l => l.PatentVerified, true)
                .Set(l => l.HasPatent, false)
                .Set(l => l.UpdatedAt, DateTime.UtcNow);

            var result = await _leadsCollection.UpdateOneAsync(
                l => l.Id == id,
                update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            return Ok(new { message = "Verificação de patente pulada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error skipping patent verification {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao pular verificação" });
        }
    }

    // POST /api/crm/leads/{id}/provision
    [HttpPost("leads/{id}/provision")]
    public async Task<ActionResult> ProvisionLead(string id)
    {
        try
        {
            var lead = await _leadsCollection
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (lead == null)
            {
                return NotFound(new { error = "Lead não encontrado" });
            }

            // TODO: Implementar lógica de provisionamento
            // - Criar usuário
            // - Criar tenant
            // - Enviar email de boas-vindas
            // - etc.

            var update = Builders<Lead>.Update
                .Set(l => l.Status, "ganho")
                .Set(l => l.DataConversao, DateTime.UtcNow)
                .Set(l => l.UpdatedAt, DateTime.UtcNow);

            await _leadsCollection.UpdateOneAsync(l => l.Id == id, update);

            _logger.LogInformation("Lead provisioned: {LeadId}", id);

            return Ok(new
            {
                message = "Lead provisionado com sucesso",
                status = "ganho"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error provisioning lead {LeadId}", id);
            return StatusCode(500, new { error = "Erro ao provisionar lead" });
        }
    }

    // POST /api/crm/leads/webhook
    [HttpPost("leads/webhook")]
    [AllowAnonymous]
    public async Task<ActionResult> LeadsWebhook([FromBody] LeadWebhookRequest request)
    {
        try
        {
            var lead = new Lead
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Empresa = request.Empresa,
                Origem = request.Origem ?? "webhook",
                Status = "novo",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _leadsCollection.InsertOneAsync(lead);

            _logger.LogInformation("Lead created via webhook: {Email}", request.Email);

            return Ok(new
            {
                message = "Lead recebido com sucesso",
                id = lead.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return StatusCode(500, new { error = "Erro ao processar webhook" });
        }
    }

    // GET /api/crm/leads/stats
    [HttpGet("leads/stats")]
    public async Task<ActionResult> GetLeadsStats()
    {
        try
        {
            var total = await _leadsCollection.CountDocumentsAsync(_ => true);
            var novos = await _leadsCollection.CountDocumentsAsync(l => l.Status == "novo");
            var contato = await _leadsCollection.CountDocumentsAsync(l => l.Status == "contato");
            var ganhos = await _leadsCollection.CountDocumentsAsync(l => l.Status == "ganho");
            var perdidos = await _leadsCollection.CountDocumentsAsync(l => l.Status == "perdido");

            return Ok(new
            {
                total,
                novos,
                contato,
                ganhos,
                perdidos,
                taxaConversao = total > 0 ? (ganhos / (double)total * 100) : 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leads stats");
            return StatusCode(500, new { error = "Erro ao buscar estatísticas" });
        }
    }

    #endregion

    #region DTOs

    public record UpdateStatusRequest(string Status);
    public record VerifyPatentRequest(string Email, bool HasPatent);
    public record LeadWebhookRequest(
        string Nome,
        string Email,
        string? Telefone,
        string? Empresa,
        string? Origem
    );

    #endregion
}
