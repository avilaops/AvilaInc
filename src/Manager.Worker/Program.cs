using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Worker.Services;
using Manager.Worker.BackgroundServices;
using Manager.Infrastructure.Repositories;
using Manager.Infrastructure.Services;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ManagerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Host=localhost;Database=manager;Username=postgres;Password=postgres"
    )
);

// Configure MongoDB
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
if (mongoSettings != null && !string.IsNullOrEmpty(mongoSettings.ConnectionString))
{
    builder.Services.AddSingleton<IMongoDbContext>(sp => 
        new MongoDbContext(mongoSettings.ConnectionString));
    builder.Services.AddSingleton(mongoSettings);

    // Website Generator Collections
    builder.Services.AddScoped(sp =>
    {
        var context = sp.GetRequiredService<IMongoDbContext>();
        var settings = sp.GetRequiredService<MongoDbSettings>();
        return context.GetCollection<Manager.Core.Entities.WebsiteRequest>(
            settings.DatabaseNames.Crm, "website_requests");
    });

    builder.Services.AddScoped(sp =>
    {
        var context = sp.GetRequiredService<IMongoDbContext>();
        var settings = sp.GetRequiredService<MongoDbSettings>();
        return context.GetCollection<Manager.Core.Entities.WebsiteProject>(
            settings.DatabaseNames.Crm, "website_projects");
    });

    builder.Services.AddScoped(sp =>
    {
        var context = sp.GetRequiredService<IMongoDbContext>();
        var settings = sp.GetRequiredService<MongoDbSettings>();
        return context.GetCollection<Manager.Core.Entities.WebsiteDeployment>(
            settings.DatabaseNames.Crm, "website_deployments");
    });

    // Repositories
    builder.Services.AddScoped<IWebsiteRequestRepository, WebsiteRequestRepository>();
    builder.Services.AddScoped<IWebsiteProjectRepository, WebsiteProjectRepository>();
    builder.Services.AddScoped<IWebsiteDeploymentRepository, WebsiteDeploymentRepository>();

    // Services
    builder.Services.AddScoped<IOpenAIService, OpenAIService>();
    builder.Services.AddScoped<IWebsiteGeneratorService, WebsiteGeneratorService>();

    // Background Workers
    builder.Services.AddHostedService<WebsiteGeneratorWorker>();
}

// Add services
builder.Services.AddScoped<IPlaybookRunner, PlaybookRunner>();

// TODO: Add background worker when build issues resolved
// builder.Services.AddHostedService<JobProcessorWorker>();

var host = builder.Build();
host.Run();
