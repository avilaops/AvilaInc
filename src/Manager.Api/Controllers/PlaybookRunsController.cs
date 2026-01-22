using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Contracts.DTOs;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaybookRunsController : ControllerBase
{
    private readonly ManagerDbContext _context;

    public PlaybookRunsController(ManagerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaybookRunResponse>>> GetPlaybookRuns([FromQuery] Guid? projectId)
    {
        var query = _context.PlaybookRuns
            .Include(r => r.Playbook)
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(r => r.ProjectId == projectId.Value);

        var runs = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new PlaybookRunResponse(
                r.Id,
                r.PlaybookId,
                r.ProjectId,
                r.Playbook!.Name,
                r.Status,
                r.CreatedAt,
                r.StartedAt,
                r.CompletedAt,
                r.ErrorMessage
            ))
            .ToListAsync();

        return Ok(runs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlaybookRunDetailResponse>> GetPlaybookRun(Guid id)
    {
        var run = await _context.PlaybookRuns
            .Include(r => r.Playbook)
            .Include(r => r.StepRuns.OrderBy(s => s.StepOrder))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (run == null)
            return NotFound();

        var response = new PlaybookRunDetailResponse(
            run.Id,
            run.PlaybookId,
            run.ProjectId,
            run.Playbook!.Name,
            run.Status,
            run.CreatedAt,
            run.StartedAt,
            run.CompletedAt,
            run.ErrorMessage,
            run.StepRuns.Select(s => new StepRunResponse(
                s.Id,
                s.StepName,
                s.StepOrder,
                s.Status,
                s.StartedAt,
                s.CompletedAt,
                s.ErrorMessage
            )).ToList()
        );

        return Ok(response);
    }
}
