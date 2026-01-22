using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Manager.Infrastructure.Data;
using System.IO;

namespace Manager.Infrastructure;

public class DbContextFactory : IDesignTimeDbContextFactory<ManagerDbContext>
{
    public ManagerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Manager.Api"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ManagerDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            "Host=localhost;Database=manager;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new ManagerDbContext(optionsBuilder.Options);
    }
}