using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Core.Entities.Projects;
using Manager.Contracts.DTOs;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ManagerDbContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(ManagerDbContext context, ILogger<ProjectsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjects([FromQuery] Guid? clientId)
    {
        var query = _context.Projects.AsQueryable();

        if (clientId.HasValue)
            query = query.Where(p => p.ClientId == clientId.Value);

        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectResponse(
                p.Id,
                p.Name,
                p.ClientId,
                p.Type,
                p.Status,
                p.Description,
                p.Domain,
                p.RepositoryUrl,
                p.DeploymentUrl,
                p.CreatedAt
            ))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailResponse>> GetProject(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Spec)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        var response = new ProjectDetailResponse(
            project.Id,
            project.Name,
            project.ClientId,
            project.Type,
            project.Status,
            project.Description,
            project.Domain,
            project.RepositoryUrl,
            project.DeploymentUrl,
            project.CreatedAt,
            project.Spec != null ? new ProjectSpecResponse(
                project.Spec.Brand,
                project.Spec.Vertical,
                project.Spec.Goal
            ) : null
        );

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> CreateProject(CreateProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name,
            ClientId = request.ClientId,
            Type = request.Type,
            Description = request.Description
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} created", project.Id);

        var response = new ProjectResponse(
            project.Id,
            project.Name,
            project.ClientId,
            project.Type,
            project.Status,
            project.Description,
            project.Domain,
            project.RepositoryUrl,
            project.DeploymentUrl,
            project.CreatedAt
        );

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectRequest request)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        if (request.Name != null) project.Name = request.Name;
        if (request.Description != null) project.Description = request.Description;
        if (request.Domain != null) project.Domain = request.Domain;

        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} updated", id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Project {ProjectId} deleted", id);

        return NoContent();
    }
}
