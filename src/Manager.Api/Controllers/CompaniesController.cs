using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Contracts.DTOs;
using Manager.Core.Entities;
using Manager.Infrastructure.Repositories;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyMongoRepository _companyRepo;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(
        ICompanyMongoRepository companyRepo,
        ILogger<CompaniesController> logger)
    {
        _companyRepo = companyRepo;
        _logger = logger;
    }

    /// <summary>
    /// Listar empresas com filtros
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? source = null,
        [FromQuery] string? query = null,
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 50)
    {
        try
        {
            var companies = await _companyRepo.GetAllAsync(source, query, skip, limit);
            var dtos = companies.Select(MapToDto).ToList();

            return Ok(new { success = true, count = dtos.Count, companies = dtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar empresas");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Obter empresa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var company = await _companyRepo.GetByIdAsync(id);
            if (company == null)
                return NotFound(new { success = false, message = "Empresa não encontrada" });

            return Ok(new { success = true, company = MapToDto(company) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar empresa");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar empresa manualmente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
    {
        try
        {
            var company = new Company
            {
                Id = null, // MongoDB irá gerar
                Source = "Manual",
                Name = request.Name,
                Cnpj = request.Cnpj,
                Address = request.Address,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website
            };

            var created = await _companyRepo.CreateAsync(company);
            return Ok(new { success = true, company = MapToDto(created) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar empresa");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar empresa
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateCompanyRequest request)
    {
        try
        {
            var company = await _companyRepo.GetByIdAsync(id);
            if (company == null)
                return NotFound(new { success = false, message = "Empresa não encontrada" });

            company.Name = request.Name;
            company.Cnpj = request.Cnpj;
            company.Address = request.Address;
            company.City = request.City;
            company.State = request.State;
            company.ZipCode = request.ZipCode;
            company.Phone = request.Phone;
            company.Email = request.Email;
            company.Website = request.Website;

            var updated = await _companyRepo.UpdateAsync(company);
            return Ok(new { success = true, company = MapToDto(updated) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar empresa");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Deletar empresa
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _companyRepo.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Empresa não encontrada" });

            return Ok(new { success = true, message = "Empresa deletada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar empresa");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    private static CompanyDto MapToDto(Company c) => new()
    {
        Id = c.Id,
        Source = c.Source,
        GooglePlaceId = c.GooglePlaceId,
        Cnpj = c.Cnpj,
        Name = c.Name,
        FantasyName = c.FantasyName,
        Address = c.Address,
        City = c.City,
        State = c.State,
        ZipCode = c.ZipCode,
        Latitude = c.Location?.Lat,
        Longitude = c.Location?.Lng,
        Phone = c.Phone,
        Email = c.Email,
        Website = c.Website,
        Types = System.Text.Json.JsonSerializer.Serialize(c.Types),
        Rating = c.Rating,
        UserRatingsTotal = c.UserRatingsTotal,
        BusinessStatus = c.BusinessStatus,
        CadastralStatus = c.CadastralStatus,
        OpeningDate = c.OpeningDate,
        CnaeMain = c.CnaeMain,
        CnaeSecondary = c.CnaeSecondary,
        LastSyncedAt = c.LastSyncedAt,
        LastCnpjLookupAt = c.LastCnpjLookupAt,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}