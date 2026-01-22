using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/leads")]
[AllowAnonymous]
public class LeadsController : ControllerBase
{
    private readonly IMongoCollection<Lead> _leadsCollection;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(
        IMongoCollection<Lead> leadsCollection,
        ILogger<LeadsController> logger)
    {
        _leadsCollection = leadsCollection;
        _logger = logger;
    }

    // GET /api/leads
    [HttpGet]
    public async Task<ActionResult> GetLeads(
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var filterBuilder = Builders<Lead>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                filter &= filterBuilder.Eq(l => l.Status, status.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(l => l.Nome, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(l => l.Email, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(l => l.Empresa, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                );
                filter &= searchFilter;
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
                success = true,
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
            return StatusCode(500, new { success = false, error = "Erro ao buscar leads" });
        }
    }

    // GET /api/leads/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetLead(string id)
    {
        try
        {
            var filter = Builders<Lead>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
            var lead = await _leadsCollection.Find(filter).FirstOrDefaultAsync();

            if (lead == null)
            {
                return NotFound(new { success = false, error = "Lead não encontrado" });
            }

            return Ok(new { success = true, data = lead });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching lead {Id}", id);
            return StatusCode(500, new { success = false, error = "Erro ao buscar lead" });
        }
    }

    // POST /api/leads
    [HttpPost]
    public async Task<ActionResult> CreateLead([FromBody] Lead lead)
    {
        try
        {
            lead.CreatedAt = DateTime.UtcNow;
            lead.UpdatedAt = DateTime.UtcNow;
            
            await _leadsCollection.InsertOneAsync(lead);

            return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, new { success = true, data = lead });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            return StatusCode(500, new { success = false, error = "Erro ao criar lead" });
        }
    }

    // PUT /api/leads/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateLead(string id, [FromBody] Lead lead)
    {
        try
        {
            var filter = Builders<Lead>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
            var existingLead = await _leadsCollection.Find(filter).FirstOrDefaultAsync();

            if (existingLead == null)
            {
                return NotFound(new { success = false, error = "Lead não encontrado" });
            }

            lead.Id = id;
            lead.CreatedAt = existingLead.CreatedAt;
            lead.UpdatedAt = DateTime.UtcNow;

            var result = await _leadsCollection.ReplaceOneAsync(filter, lead);

            if (result.ModifiedCount == 0)
            {
                return BadRequest(new { success = false, error = "Erro ao atualizar lead" });
            }

            return Ok(new { success = true, data = lead });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead {Id}", id);
            return StatusCode(500, new { success = false, error = "Erro ao atualizar lead" });
        }
    }

    // DELETE /api/leads/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLead(string id)
    {
        try
        {
            var filter = Builders<Lead>.Filter.Eq("_id", MongoDB.Bson.ObjectId.Parse(id));
            var result = await _leadsCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { success = false, error = "Lead não encontrado" });
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead {Id}", id);
            return StatusCode(500, new { success = false, error = "Erro ao excluir lead" });
        }
    }

    // GET /api/leads/stats
    [HttpGet("stats")]
    public async Task<ActionResult> GetLeadsStats()
    {
        try
        {
            var total = await _leadsCollection.CountDocumentsAsync(_ => true);
            
            var statusPipeline = new[]
            {
                new MongoDB.Bson.BsonDocument("$group", new MongoDB.Bson.BsonDocument
                {
                    { "_id", "$status" },
                    { "count", new MongoDB.Bson.BsonDocument("$sum", 1) }
                })
            };

            var statusStats = await _leadsCollection
                .Aggregate<MongoDB.Bson.BsonDocument>(statusPipeline)
                .ToListAsync();

            var stats = statusStats.Select(s => new
            {
                status = s["_id"].AsString,
                count = s["count"].AsInt32
            }).ToList();

            return Ok(new
            {
                success = true,
                data = new
                {
                    total,
                    byStatus = stats
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching leads stats");
            return StatusCode(500, new { success = false, error = "Erro ao buscar estatísticas" });
        }
    }
}
