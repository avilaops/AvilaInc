using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
[AllowAnonymous]
public class CampaignsController : ControllerBase
{
    private readonly IMongoCollection<Campaign> _campaignsCollection;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(
        IMongoCollection<Campaign> campaignsCollection,
        ILogger<CampaignsController> logger)
    {
        _campaignsCollection = campaignsCollection;
        _logger = logger;
    }

    // GET /api/campanhas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns(
        [FromQuery] string? status = null,
        [FromQuery] string? tipo = null)
    {
        try
        {
            var filterBuilder = Builders<Campaign>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter &= filterBuilder.Eq(c => c.Status, status);
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                filter &= filterBuilder.Eq(c => c.Tipo, tipo);
            }

            var campaigns = await _campaignsCollection
                .Find(filter)
                .Sort(Builders<Campaign>.Sort.Descending(c => c.CreatedAt))
                .ToListAsync();

            return Ok(new { success = true, data = campaigns });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching campaigns");
            return StatusCode(500, new { error = "Erro ao buscar campanhas" });
        }
    }

    // GET /api/campanhas/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Campaign>> GetCampaign(string id)
    {
        try
        {
            var campaign = await _campaignsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (campaign == null)
            {
                return NotFound(new { error = "Campanha não encontrada" });
            }

            return Ok(campaign);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching campaign {CampaignId}", id);
            return StatusCode(500, new { error = "Erro ao buscar campanha" });
        }
    }

    // POST /api/campanhas
    [HttpPost]
    public async Task<ActionResult<Campaign>> CreateCampaign([FromBody] Campaign campaign)
    {
        try
        {
            campaign.CreatedAt = DateTime.UtcNow;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _campaignsCollection.InsertOneAsync(campaign);

            _logger.LogInformation("Campaign created: {CampaignId}", campaign.Id);

            return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign");
            return StatusCode(500, new { error = "Erro ao criar campanha" });
        }
    }

    // PUT /api/campanhas/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Campaign>> UpdateCampaign(string id, [FromBody] Campaign campaign)
    {
        try
        {
            var existingCampaign = await _campaignsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (existingCampaign == null)
            {
                return NotFound(new { error = "Campanha não encontrada" });
            }

            campaign.Id = id;
            campaign.CreatedAt = existingCampaign.CreatedAt;
            campaign.UpdatedAt = DateTime.UtcNow;

            await _campaignsCollection.ReplaceOneAsync(c => c.Id == id, campaign);

            _logger.LogInformation("Campaign updated: {CampaignId}", id);

            return Ok(campaign);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign {CampaignId}", id);
            return StatusCode(500, new { error = "Erro ao atualizar campanha" });
        }
    }

    // DELETE /api/campanhas/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCampaign(string id)
    {
        try
        {
            var result = await _campaignsCollection.DeleteOneAsync(c => c.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { error = "Campanha não encontrada" });
            }

            _logger.LogInformation("Campaign deleted: {CampaignId}", id);

            return Ok(new { message = "Campanha removida com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting campaign {CampaignId}", id);
            return StatusCode(500, new { error = "Erro ao remover campanha" });
        }
    }

    // GET /api/campanhas/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Campaign>>> GetActiveCampaigns()
    {
        try
        {
            var now = DateTime.UtcNow;
            var filter = Builders<Campaign>.Filter.And(
                Builders<Campaign>.Filter.Eq(c => c.Status, "ativa"),
                Builders<Campaign>.Filter.Lte(c => c.DataInicio, now),
                Builders<Campaign>.Filter.Or(
                    Builders<Campaign>.Filter.Eq(c => c.DataFim, null),
                    Builders<Campaign>.Filter.Gte(c => c.DataFim, now)
                )
            );

            var campaigns = await _campaignsCollection
                .Find(filter)
                .ToListAsync();

            return Ok(campaigns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active campaigns");
            return StatusCode(500, new { error = "Erro ao buscar campanhas ativas" });
        }
    }

    // GET /api/campanhas/{id}/stats
    [HttpGet("{id}/stats")]
    public async Task<ActionResult> GetCampaignStats(string id)
    {
        try
        {
            var campaign = await _campaignsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (campaign == null)
            {
                return NotFound(new { error = "Campanha não encontrada" });
            }

            var taxaAbertura = campaign.Enviados > 0 
                ? (campaign.Abertos / (double)campaign.Enviados * 100) 
                : 0;

            var taxaClique = campaign.Abertos > 0 
                ? (campaign.Cliques / (double)campaign.Abertos * 100) 
                : 0;

            var taxaConversao = campaign.Enviados > 0 
                ? (campaign.Conversoes / (double)campaign.Enviados * 100) 
                : 0;

            return Ok(new
            {
                destinatarios = campaign.Destinatarios,
                enviados = campaign.Enviados,
                abertos = campaign.Abertos,
                cliques = campaign.Cliques,
                conversoes = campaign.Conversoes,
                taxaAbertura = Math.Round(taxaAbertura, 2),
                taxaClique = Math.Round(taxaClique, 2),
                taxaConversao = Math.Round(taxaConversao, 2)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign stats {CampaignId}", id);
            return StatusCode(500, new { error = "Erro ao buscar estatísticas" });
        }
    }

    // POST /api/campanhas/{id}/send
    [HttpPost("{id}/send")]
    public async Task<ActionResult> SendCampaign(string id)
    {
        try
        {
            var campaign = await _campaignsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (campaign == null)
            {
                return NotFound(new { error = "Campanha não encontrada" });
            }

            // TODO: Implementar lógica de envio de campanha
            // - Buscar destinatários
            // - Enviar emails
            // - Atualizar contadores

            var update = Builders<Campaign>.Update
                .Set(c => c.Status, "enviada")
                .Set(c => c.DataInicio, DateTime.UtcNow)
                .Set(c => c.UpdatedAt, DateTime.UtcNow);

            await _campaignsCollection.UpdateOneAsync(c => c.Id == id, update);

            _logger.LogInformation("Campaign sent: {CampaignId}", id);

            return Ok(new { message = "Campanha enviada com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending campaign {CampaignId}", id);
            return StatusCode(500, new { error = "Erro ao enviar campanha" });
        }
    }
}
