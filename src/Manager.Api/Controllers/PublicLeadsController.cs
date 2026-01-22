using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/public/leads")]
public class PublicLeadsController : ControllerBase
{
    private readonly IMongoCollection<Lead> _leadsCollection;
    private readonly ILogger<PublicLeadsController> _logger;

    public PublicLeadsController(
        IMongoCollection<Lead> leadsCollection,
        ILogger<PublicLeadsController> logger)
    {
        _leadsCollection = leadsCollection;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint público para captura de leads da Landing Page
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var lead = new Lead
            {
                Nome = request.Name.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                Telefone = request.Phone.Trim(),
                Observacoes = request.Message?.Trim(),
                Origem = request.Source ?? "Landing",
                Status = "novo",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _leadsCollection.InsertOneAsync(lead);

            _logger.LogInformation(
                "Lead criado: {Email} - {Name} via {Source}",
                lead.Email,
                lead.Nome,
                lead.Origem
            );

            return Ok(new { ok = true, id = lead.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar lead público");
            return StatusCode(500, new { ok = false, error = "Erro interno do servidor" });
        }
    }
}

/// <summary>
/// DTO para criação de lead público (Landing Page)
/// </summary>
public sealed class CreateLeadRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "Nome muito longo")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(150, ErrorMessage = "Email muito longo")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [MinLength(8, ErrorMessage = "Telefone deve ter pelo menos 8 dígitos")]
    [MaxLength(20, ErrorMessage = "Telefone muito longo")]
    [Phone(ErrorMessage = "Formato de telefone inválido")]
    public string Phone { get; set; } = "";

    [MaxLength(500, ErrorMessage = "Mensagem muito longa")]
    public string? Message { get; set; }

    [MaxLength(50)]
    public string Source { get; set; } = "Landing";
}
