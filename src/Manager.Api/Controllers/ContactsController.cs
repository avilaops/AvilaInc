using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;
using System.Text;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[AllowAnonymous]
public class ContactsController : ControllerBase
{
    private readonly IMongoCollection<Contact> _contactsCollection;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(
        IMongoCollection<Contact> contactsCollection,
        ILogger<ContactsController> logger)
    {
        _contactsCollection = contactsCollection;
        _logger = logger;
    }

    // GET /api/contacts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetContacts(
        [FromQuery] string? source = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var filterBuilder = Builders<Contact>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                filter &= filterBuilder.Eq(c => c.Source, source);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(c => c.Email, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(c => c.Company, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                );
                filter &= searchFilter;
            }

            var contacts = await _contactsCollection
                .Find(filter)
                .Sort(Builders<Contact>.Sort.Descending(c => c.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            var total = await _contactsCollection.CountDocumentsAsync(filter);

            return Ok(new
            {
                success = true,
                data = contacts,
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
            _logger.LogError(ex, "Error fetching contacts");
            return StatusCode(500, new { success = false, error = "Erro ao buscar contatos" });
        }
    }

    // GET /api/contacts/unified
    [HttpGet("unified")]
    public async Task<ActionResult<IEnumerable<Contact>>> GetUnifiedContacts(
        [FromQuery] string? source = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var filterBuilder = Builders<Contact>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                filter &= filterBuilder.Eq(c => c.Source, source);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(c => c.Email, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    filterBuilder.Regex(c => c.Company, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                );
                filter &= searchFilter;
            }

            var contacts = await _contactsCollection
                .Find(filter)
                .Sort(Builders<Contact>.Sort.Descending(c => c.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            var total = await _contactsCollection.CountDocumentsAsync(filter);

            return Ok(new
            {
                data = contacts,
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
            _logger.LogError(ex, "Error fetching unified contacts");
            return StatusCode(500, new { error = "Erro ao buscar contatos" });
        }
    }

    // GET /api/contacts/stats
    [HttpGet("stats")]
    public async Task<ActionResult> GetContactsStats()
    {
        try
        {
            var total = await _contactsCollection.CountDocumentsAsync(_ => true);
            
            var sourcesPipeline = new[]
            {
                new MongoDB.Bson.BsonDocument("$group", new MongoDB.Bson.BsonDocument
                {
                    { "_id", "$source" },
                    { "count", new MongoDB.Bson.BsonDocument("$sum", 1) }
                })
            };

            var sourceStats = await _contactsCollection
                .Aggregate<MongoDB.Bson.BsonDocument>(sourcesPipeline)
                .ToListAsync();

            var sources = sourceStats.ToDictionary(
                doc => doc["_id"].AsString,
                doc => doc["count"].AsInt32
            );

            return Ok(new
            {
                total,
                sources
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contacts stats");
            return StatusCode(500, new { error = "Erro ao buscar estatísticas" });
        }
    }

    // GET /api/contacts/export/csv
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportContactsToCSV([FromQuery] string? source = null)
    {
        try
        {
            var filterBuilder = Builders<Contact>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                filter &= filterBuilder.Eq(c => c.Source, source);
            }

            var contacts = await _contactsCollection
                .Find(filter)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Nome,Email,Telefone,Empresa,Cargo,Origem,Status,LinkedIn,Data de Criação");

            foreach (var contact in contacts)
            {
                csv.AppendLine($"\"{contact.Name}\",\"{contact.Email}\",\"{contact.Phone}\",\"{contact.Company}\",\"{contact.Position}\",\"{contact.Source}\",\"{contact.Status}\",\"{contact.LinkedInUrl}\",\"{contact.CreatedAt:yyyy-MM-dd}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"contatos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting contacts to CSV");
            return StatusCode(500, new { error = "Erro ao exportar contatos" });
        }
    }

    // POST /api/contacts
    [HttpPost]
    public async Task<ActionResult<Contact>> CreateContact([FromBody] Contact contact)
    {
        try
        {
            // Check if contact already exists
            var existingContact = await _contactsCollection
                .Find(c => c.Email == contact.Email)
                .FirstOrDefaultAsync();

            if (existingContact != null)
            {
                return Conflict(new { error = "Contato já existe com este email" });
            }

            contact.CreatedAt = DateTime.UtcNow;
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactsCollection.InsertOneAsync(contact);

            _logger.LogInformation("Contact created: {Email}", contact.Email);

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact");
            return StatusCode(500, new { error = "Erro ao criar contato" });
        }
    }

    // GET /api/contacts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContact(string id)
    {
        try
        {
            var contact = await _contactsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound(new { error = "Contato não encontrado" });
            }

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching contact {ContactId}", id);
            return StatusCode(500, new { error = "Erro ao buscar contato" });
        }
    }

    // PUT /api/contacts/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Contact>> UpdateContact(string id, [FromBody] Contact contact)
    {
        try
        {
            var existingContact = await _contactsCollection
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (existingContact == null)
            {
                return NotFound(new { error = "Contato não encontrado" });
            }

            contact.Id = id;
            contact.CreatedAt = existingContact.CreatedAt;
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactsCollection.ReplaceOneAsync(c => c.Id == id, contact);

            _logger.LogInformation("Contact updated: {ContactId}", id);

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {ContactId}", id);
            return StatusCode(500, new { error = "Erro ao atualizar contato" });
        }
    }

    // DELETE /api/contacts/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteContact(string id)
    {
        try
        {
            var result = await _contactsCollection.DeleteOneAsync(c => c.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { error = "Contato não encontrado" });
            }

            _logger.LogInformation("Contact deleted: {ContactId}", id);

            return Ok(new { message = "Contato removido com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {ContactId}", id);
            return StatusCode(500, new { error = "Erro ao remover contato" });
        }
    }

    // POST /api/contacts/import
    [HttpPost("import")]
    public async Task<ActionResult> ImportContacts([FromBody] ImportContactsRequest request)
    {
        try
        {
            var contacts = request.Contacts.Select(c => new Contact
            {
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Company = c.Company,
                Position = c.Position,
                Source = request.Source ?? "import",
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImportDate = DateTime.UtcNow
            }).ToList();

            await _contactsCollection.InsertManyAsync(contacts);

            _logger.LogInformation("Imported {Count} contacts from {Source}", contacts.Count, request.Source);

            return Ok(new
            {
                message = "Contatos importados com sucesso",
                count = contacts.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing contacts");
            return StatusCode(500, new { error = "Erro ao importar contatos" });
        }
    }

    #region DTOs

    public record ImportContactsRequest(
        List<ContactDto> Contacts,
        string? Source
    );

    public record ContactDto(
        string Name,
        string Email,
        string? Phone,
        string? Company,
        string? Position
    );

    #endregion
}
