using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Core.Entities.Clients;
using Manager.Contracts.DTOs;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ManagerDbContext _context;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(ManagerDbContext context, ILogger<ClientsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetClients()
    {
        var clients = await _context.Clients
            .OrderBy(c => c.Name)
            .Select(c => new ClientResponse(
                c.Id,
                c.Name,
                c.TaxId,
                c.Vertical,
                c.IsActive,
                c.CreatedAt
            ))
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponse>> GetClient(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        var response = new ClientResponse(
            client.Id,
            client.Name,
            client.TaxId,
            client.Vertical,
            client.IsActive,
            client.CreatedAt
        );

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> CreateClient(CreateClientRequest request)
    {
        var client = new Client
        {
            Name = request.Name,
            TaxId = request.TaxId,
            Vertical = request.Vertical
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Client {ClientId} created", client.Id);

        var response = new ClientResponse(
            client.Id,
            client.Name,
            client.TaxId,
            client.Vertical,
            client.IsActive,
            client.CreatedAt
        );

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, response);
    }
}
