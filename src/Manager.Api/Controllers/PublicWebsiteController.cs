using Manager.Contracts.DTOs;
using Manager.Core.Entities;
using Manager.Core.Enums;
using Manager.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/public/website-requests")]
public sealed class PublicWebsiteController : ControllerBase
{
    private readonly IWebsiteRequestRepository _requestRepo;
    private readonly IWebsiteProjectRepository _projectRepo;
    private readonly ILogger<PublicWebsiteController> _logger;

    public PublicWebsiteController(
        IWebsiteRequestRepository requestRepo,
        IWebsiteProjectRepository projectRepo,
        ILogger<PublicWebsiteController> logger)
    {
        _requestRepo = requestRepo;
        _projectRepo = projectRepo;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo pedido de website
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebsiteRequestResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateRequest([FromBody] CreateWebsiteRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            // Rate limiting check (exemplo simples por IP)
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var recentRequests = await _requestRepo.FindAsync(x =>
                x.CustomerIp == clientIp &&
                x.CreatedAt > DateTime.UtcNow.AddHours(-1));

            if (recentRequests.Count() >= 3)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    new { error = "Limite de requisições atingido. Tente novamente mais tarde." });
            }

            // Sanitizar inputs
            var request = new WebsiteRequest
            {
                BusinessName = SanitizeInput(dto.BusinessName),
                Niche = SanitizeInput(dto.Niche),
                City = SanitizeInput(dto.City),
                Services = dto.Services.Select(SanitizeInput).ToList(),
                Differentials = SanitizeInput(dto.Differentials),
                WhatsApp = SanitizePhoneNumber(dto.WhatsApp),
                Email = dto.Email.Trim().ToLowerInvariant(),
                TemplateType = dto.TemplateType,
                ColorPreference = dto.ColorPreference != null ? SanitizeInput(dto.ColorPreference) : null,
                LogoUrl = dto.LogoUrl,
                Status = WebsiteRequestStatus.Received,
                CustomerIp = clientIp,
                CustomerUserAgent = Request.Headers.UserAgent.ToString()
            };

            var created = await _requestRepo.CreateAsync(request);

            _logger.LogInformation(
                "Website request created: {RequestId} for {BusinessName}",
                created.Id, created.BusinessName);

            var response = new WebsiteRequestResponseDto
            {
                Id = created.Id!,
                BusinessName = created.BusinessName,
                Status = created.Status,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            };

            return CreatedAtAction(nameof(GetRequestStatus), new { id = created.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating website request");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Erro ao processar pedido. Tente novamente." });
        }
    }

    /// <summary>
    /// Consulta o status de um pedido
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WebsiteRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRequestStatus(string id)
    {
        var request = await _requestRepo.GetByIdAsync(id);
        if (request == null)
        {
            return NotFound(new { error = "Pedido não encontrado" });
        }

        WebsiteProject? project = null;
        if (request.ProjectId != null)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId);
        }

        var response = new WebsiteRequestResponseDto
        {
            Id = request.Id!,
            BusinessName = request.BusinessName,
            Status = request.Status,
            PreviewUrl = project?.PreviewUrl,
            LiveUrl = project?.LiveUrl,
            ErrorMessage = request.ErrorMessage,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtém preview do projeto gerado
    /// </summary>
    [HttpGet("{id}/preview")]
    [ProducesResponseType(typeof(WebsiteProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPreview(string id)
    {
        var request = await _requestRepo.GetByIdAsync(id);
        if (request == null || request.ProjectId == null)
        {
            return NotFound(new { error = "Preview não disponível" });
        }

        var project = await _projectRepo.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return NotFound(new { error = "Projeto não encontrado" });
        }

        var response = new WebsiteProjectDto
        {
            Id = project.Id!,
            Subdomain = project.Subdomain,
            PreviewUrl = project.PreviewUrl ?? "",
            LiveUrl = project.LiveUrl ?? "",
            IsPublished = project.IsPublished,
            Content = new WebsiteContentDto
            {
                BusinessName = project.Content.BusinessName,
                HeroHeadline = project.Content.HeroHeadline,
                HeroSubheadline = project.Content.HeroSubheadline,
                Services = project.Content.Services.Select(s => new ServiceItemDto(s.Icon, s.Title, s.Description)).ToList(),
                Benefits = project.Content.Benefits,
                FaqItems = project.Content.FaqItems.Select(f => new FaqItemDto(f.Question, f.Answer)).ToList(),
                WhatsApp = project.Content.WhatsApp,
                Email = project.Content.Email
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Publica o website (torna público)
    /// </summary>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishWebsite(string id)
    {
        var request = await _requestRepo.GetByIdAsync(id);
        if (request == null || request.ProjectId == null)
        {
            return NotFound(new { error = "Pedido não encontrado" });
        }

        if (request.Status != WebsiteRequestStatus.ReadyForReview)
        {
            return BadRequest(new { error = "Website ainda não está pronto para publicação" });
        }

        var project = await _projectRepo.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return NotFound(new { error = "Projeto não encontrado" });
        }

        if (project.IsPublished)
        {
            return Ok(new { message = "Website já está publicado", liveUrl = project.LiveUrl });
        }

        try
        {
            // Aqui seria o trigger para o Worker fazer o deploy final
            // Por enquanto, apenas atualiza o status
            await _requestRepo.UpdateStatusAsync(id, WebsiteRequestStatus.Published);

            var liveUrl = $"https://{project.Subdomain}.avila.inc";
            await _projectRepo.PublishAsync(request.ProjectId, liveUrl);

            _logger.LogInformation("Website published: {ProjectId} -> {LiveUrl}", request.ProjectId, liveUrl);

            return Ok(new { message = "Website publicado com sucesso", liveUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing website {ProjectId}", request.ProjectId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Erro ao publicar website" });
        }
    }

    private static string SanitizeInput(string input)
    {
        return input
            .Trim()
            .Replace("<", "")
            .Replace(">", "")
            .Replace("'", "")
            .Replace("\"", "");
    }

    private static string SanitizePhoneNumber(string phone)
    {
        return new string(phone.Where(char.IsDigit).ToArray());
    }
}
