using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/gestores")]
[Authorize]
public class GestoresController : ControllerBase
{
    private readonly IMongoCollection<Gestor> _gestoresCollection;
    private readonly ILogger<GestoresController> _logger;

    public GestoresController(
        IMongoCollection<Gestor> gestoresCollection,
        ILogger<GestoresController> logger)
    {
        _gestoresCollection = gestoresCollection;
        _logger = logger;
    }

    // GET /api/gestores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gestor>>> GetGestores([FromQuery] bool? ativo = null)
    {
        try
        {
            var filterBuilder = Builders<Gestor>.Filter;
            var filter = filterBuilder.Empty;

            if (ativo.HasValue)
            {
                filter &= filterBuilder.Eq(g => g.Ativo, ativo.Value);
            }

            var gestores = await _gestoresCollection
                .Find(filter)
                .Sort(Builders<Gestor>.Sort.Ascending(g => g.Nome))
                .ToListAsync();

            return Ok(gestores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching gestores");
            return StatusCode(500, new { error = "Erro ao buscar gestores" });
        }
    }

    // GET /api/gestores/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Gestor>> GetGestor(string id)
    {
        try
        {
            var gestor = await _gestoresCollection
                .Find(g => g.Id == id)
                .FirstOrDefaultAsync();

            if (gestor == null)
            {
                return NotFound(new { error = "Gestor não encontrado" });
            }

            return Ok(gestor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching gestor {GestorId}", id);
            return StatusCode(500, new { error = "Erro ao buscar gestor" });
        }
    }

    // POST /api/gestores
    [HttpPost]
    public async Task<ActionResult<Gestor>> CreateGestor([FromBody] Gestor gestor)
    {
        try
        {
            // Check if email already exists
            var existingGestor = await _gestoresCollection
                .Find(g => g.Email == gestor.Email)
                .FirstOrDefaultAsync();

            if (existingGestor != null)
            {
                return Conflict(new { error = "Já existe um gestor com este email" });
            }

            gestor.CreatedAt = DateTime.UtcNow;
            gestor.UpdatedAt = DateTime.UtcNow;

            await _gestoresCollection.InsertOneAsync(gestor);

            _logger.LogInformation("Gestor created: {Email}", gestor.Email);

            return CreatedAtAction(nameof(GetGestor), new { id = gestor.Id }, gestor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gestor");
            return StatusCode(500, new { error = "Erro ao criar gestor" });
        }
    }

    // PUT /api/gestores/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Gestor>> UpdateGestor(string id, [FromBody] Gestor gestor)
    {
        try
        {
            var existingGestor = await _gestoresCollection
                .Find(g => g.Id == id)
                .FirstOrDefaultAsync();

            if (existingGestor == null)
            {
                return NotFound(new { error = "Gestor não encontrado" });
            }

            gestor.Id = id;
            gestor.CreatedAt = existingGestor.CreatedAt;
            gestor.UpdatedAt = DateTime.UtcNow;

            await _gestoresCollection.ReplaceOneAsync(g => g.Id == id, gestor);

            _logger.LogInformation("Gestor updated: {GestorId}", id);

            return Ok(gestor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gestor {GestorId}", id);
            return StatusCode(500, new { error = "Erro ao atualizar gestor" });
        }
    }

    // DELETE /api/gestores/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGestor(string id)
    {
        try
        {
            var result = await _gestoresCollection.DeleteOneAsync(g => g.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { error = "Gestor não encontrado" });
            }

            _logger.LogInformation("Gestor deleted: {GestorId}", id);

            return Ok(new { message = "Gestor removido com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gestor {GestorId}", id);
            return StatusCode(500, new { error = "Erro ao remover gestor" });
        }
    }

    // POST /api/gestores/{id}/deactivate
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateGestor(string id)
    {
        try
        {
            var update = Builders<Gestor>.Update
                .Set(g => g.Ativo, false)
                .Set(g => g.UpdatedAt, DateTime.UtcNow);

            var result = await _gestoresCollection.UpdateOneAsync(
                g => g.Id == id,
                update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { error = "Gestor não encontrado" });
            }

            _logger.LogInformation("Gestor deactivated: {GestorId}", id);

            return Ok(new { message = "Gestor desativado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating gestor {GestorId}", id);
            return StatusCode(500, new { error = "Erro ao desativar gestor" });
        }
    }

    // POST /api/gestores/{id}/activate
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateGestor(string id)
    {
        try
        {
            var update = Builders<Gestor>.Update
                .Set(g => g.Ativo, true)
                .Set(g => g.UpdatedAt, DateTime.UtcNow);

            var result = await _gestoresCollection.UpdateOneAsync(
                g => g.Id == id,
                update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { error = "Gestor não encontrado" });
            }

            _logger.LogInformation("Gestor activated: {GestorId}", id);

            return Ok(new { message = "Gestor ativado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating gestor {GestorId}", id);
            return StatusCode(500, new { error = "Erro ao ativar gestor" });
        }
    }
}
