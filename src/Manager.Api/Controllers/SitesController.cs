using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/sites")]
[Authorize]
public class SitesController : ControllerBase
{
    private readonly IMongoCollection<Site> _sitesCollection;
    private readonly ILogger<SitesController> _logger;

    public SitesController(
        IMongoCollection<Site> sitesCollection,
        ILogger<SitesController> logger)
    {
        _sitesCollection = sitesCollection;
        _logger = logger;
    }

    // GET /api/sites
    [HttpGet]
    public async Task<ActionResult> GetSites(
        [FromQuery] string? search = null,
        [FromQuery] bool? active = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var filterBuilder = Builders<Site>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(s => s.Name, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(s => s.Domain, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                );
                filter &= searchFilter;
            }

            if (active.HasValue)
            {
                filter &= filterBuilder.Eq(s => s.Active, active.Value);
            }

            var total = await _sitesCollection.CountDocumentsAsync(filter);
            var sites = await _sitesCollection
                .Find(filter)
                .SortByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = sites,
                pagination = new
                {
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sites");
            return StatusCode(500, new { success = false, error = "Erro ao buscar sites" });
        }
    }

    // GET /api/sites/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetSite(string id)
    {
        try
        {
            var site = await _sitesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (site == null)
            {
                return NotFound(new { success = false, error = "Site não encontrado" });
            }

            return Ok(new { success = true, data = site });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting site {SiteId}", id);
            return StatusCode(500, new { success = false, error = "Erro ao buscar site" });
        }
    }

    // POST /api/sites
    [HttpPost]
    public async Task<ActionResult> CreateSite([FromBody] CreateSiteRequest request)
    {
        try
        {
            // Check if domain already exists
            var existing = await _sitesCollection
                .Find(s => s.Domain == request.Domain)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return BadRequest(new { success = false, error = "Domínio já cadastrado" });
            }

            var site = new Site
            {
                Name = request.Name,
                Domain = request.Domain,
                SiteId = Guid.NewGuid().ToString(),
                TrackingCode = GenerateTrackingCode(),
                Active = true,
                ClientId = request.ClientId,
                AllowedDomains = request.AllowedDomains ?? new List<string> { request.Domain },
                ChatEnabled = request.ChatEnabled,
                ChatWidget = request.ChatEnabled ? new ChatWidget() : null
            };

            await _sitesCollection.InsertOneAsync(site);

            return CreatedAtAction(nameof(GetSite), new { id = site.Id }, new { success = true, data = site });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating site");
            return StatusCode(500, new { success = false, error = "Erro ao criar site" });
        }
    }

    // PUT /api/sites/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSite(string id, [FromBody] UpdateSiteRequest request)
    {
        try
        {
            var site = await _sitesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (site == null)
            {
                return NotFound(new { success = false, error = "Site não encontrado" });
            }

            if (!string.IsNullOrEmpty(request.Name))
                site.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Domain))
                site.Domain = request.Domain;

            if (request.Active.HasValue)
                site.Active = request.Active.Value;

            if (request.AllowedDomains != null)
                site.AllowedDomains = request.AllowedDomains;

            if (request.ChatEnabled.HasValue)
            {
                site.ChatEnabled = request.ChatEnabled.Value;
                if (site.ChatEnabled && site.ChatWidget == null)
                {
                    site.ChatWidget = new ChatWidget();
                }
            }

            if (request.ChatWidget != null)
            {
                site.ChatWidget = request.ChatWidget;
            }

            site.UpdatedAt = DateTime.UtcNow;

            await _sitesCollection.ReplaceOneAsync(s => s.Id == id, site);

            return Ok(new { success = true, data = site });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating site {SiteId}", id);
            return StatusCode(500, new { success = false, error = "Erro ao atualizar site" });
        }
    }

    // DELETE /api/sites/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSite(string id)
    {
        try
        {
            var result = await _sitesCollection.DeleteOneAsync(s => s.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { success = false, error = "Site não encontrado" });
            }

            return Ok(new { success = true, message = "Site deletado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting site {SiteId}", id);
            return StatusCode(500, new { success = false, error = "Erro ao deletar site" });
        }
    }

    // POST /api/sites/{id}/regenerate-code
    [HttpPost("{id}/regenerate-code")]
    public async Task<ActionResult> RegenerateTrackingCode(string id)
    {
        try
        {
            var site = await _sitesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

            if (site == null)
            {
                return NotFound(new { success = false, error = "Site não encontrado" });
            }

            site.TrackingCode = GenerateTrackingCode();
            site.UpdatedAt = DateTime.UtcNow;

            await _sitesCollection.ReplaceOneAsync(s => s.Id == id, site);

            return Ok(new { success = true, data = site });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating tracking code for site {SiteId}", id);
            return StatusCode(500, new { success = false, error = "Erro ao regenerar código" });
        }
    }

    private string GenerateTrackingCode()
    {
        return "AVA_" + Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
    }
}

// Request DTOs
public class CreateSiteRequest
{
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? ClientId { get; set; }
    public List<string>? AllowedDomains { get; set; }
    public bool ChatEnabled { get; set; } = false;
}

public class UpdateSiteRequest
{
    public string? Name { get; set; }
    public string? Domain { get; set; }
    public bool? Active { get; set; }
    public List<string>? AllowedDomains { get; set; }
    public bool? ChatEnabled { get; set; }
    public ChatWidget? ChatWidget { get; set; }
}
