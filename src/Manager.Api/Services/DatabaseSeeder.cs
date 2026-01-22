using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Integrations.Playbooks;

namespace Manager.Api.Services;

public class DatabaseSeeder
{
    private readonly ManagerDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ManagerDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Seed playbooks if not exist
            await SeedPlaybooksAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private async Task SeedPlaybooksAsync()
    {
        var existingPlaybooks = await _context.Playbooks.ToListAsync();

        if (!existingPlaybooks.Any())
        {
            _logger.LogInformation("Seeding playbooks...");

            var playbooks = PlaybookSeeds.GetAllPlaybooks();

            _context.Playbooks.AddRange(playbooks);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} playbooks", playbooks.Count);
        }
        else
        {
            _logger.LogInformation("Playbooks already exist, skipping seed");
        }
    }
}
