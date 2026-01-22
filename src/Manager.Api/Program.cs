using Microsoft.EntityFrameworkCore;
using Manager.Infrastructure.Data;
using Manager.Api.Services;
using Manager.Infrastructure.Repositories;
using Manager.Integrations.Google;
using Manager.Integrations.Cnpj;
using MongoDB.Driver;
using Manager.Api.HealthChecks;
using Manager.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString))
{
    throw new InvalidOperationException("MongoDB configuration is missing");
}

builder.Services.AddSingleton<IMongoDbContext>(sp => 
    new MongoDbContext(mongoSettings.ConnectionString));

builder.Services.AddSingleton(mongoSettings);

// Register MongoDB repositories
builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Lead>(
        settings.DatabaseNames.Crm, "leads");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Contact>(
        settings.DatabaseNames.Crm, "contacts");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Email>(
        settings.DatabaseNames.Gmail, "emails");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Campaign>(
        settings.DatabaseNames.Crm, "campanhas");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Book>(
        settings.DatabaseNames.Dashboard, "books");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.DiaryEntry>(
        settings.DatabaseNames.Dashboard, "diary");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Gestor>(
        settings.DatabaseNames.Dashboard, "gestores");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.ClientFinance>(
        settings.DatabaseNames.Dashboard, "client_finance");
});

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

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.ClientContract>(
        settings.DatabaseNames.Dashboard, "client_contracts");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.ClientHistory>(
        settings.DatabaseNames.Dashboard, "client_history");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Company>(
        settings.DatabaseNames.Crm, "companies");
});

// Register Analytics Collections
builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.PageView>(
        settings.DatabaseNames.Dashboard, "pageviews");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.AnalyticsEvent>(
        settings.DatabaseNames.Dashboard, "analytics_events");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Session>(
        settings.DatabaseNames.Dashboard, "sessions");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Visitor>(
        settings.DatabaseNames.Dashboard, "visitors");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.Site>(
        settings.DatabaseNames.Dashboard, "sites");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.ChatMessage>(
        settings.DatabaseNames.Dashboard, "chat_messages");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.ConversionFunnel>(
        settings.DatabaseNames.Dashboard, "conversion_funnels");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.FunnelProgress>(
        settings.DatabaseNames.Dashboard, "funnel_progress");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.HeatmapClick>(
        settings.DatabaseNames.Dashboard, "heatmap_clicks");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.HeatmapScroll>(
        settings.DatabaseNames.Dashboard, "heatmap_scroll");
});

builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IMongoDbContext>();
    var settings = sp.GetRequiredService<MongoDbSettings>();
    return context.GetCollection<Manager.Core.Entities.HeatmapMove>(
        settings.DatabaseNames.Dashboard, "heatmap_moves");
});

// Keep EF Core for legacy tables (optional - can be removed later)
builder.Services.AddDbContext<ManagerDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            "Data Source=manager.db",
            b => b.MigrationsAssembly("Manager.Api")
        );
    }
    else
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            "Host=localhost;Database=manager;Username=postgres;Password=postgres",
            b => b.MigrationsAssembly("Manager.Api")
        );
    }
});

// builder.Services.AddScoped<DatabaseSeeder>();

// Register repositories
builder.Services.AddScoped<ICompanyMongoRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// Register Website Generator repositories
builder.Services.AddScoped<IWebsiteRequestRepository, WebsiteRequestRepository>();
builder.Services.AddScoped<IWebsiteProjectRepository, WebsiteProjectRepository>();
builder.Services.AddScoped<IWebsiteDeploymentRepository, WebsiteDeploymentRepository>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register Stripe service
builder.Services.AddScoped<Manager.Infrastructure.Services.IStripeService, Manager.Infrastructure.Services.StripeService>();

// Register Gmail service
builder.Services.AddScoped<Manager.Infrastructure.Services.IGmailService, Manager.Infrastructure.Services.GmailService>();

// Register OpenAI service
builder.Services.AddScoped<Manager.Infrastructure.Services.IOpenAIService, Manager.Infrastructure.Services.OpenAIService>();

// Register Google Places service
builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

// Register CNPJ lookup provider
builder.Services.AddHttpClient<ICnpjLookupProvider, HttpCnpjLookupProvider>();

// Register Company repository
builder.Services.AddScoped<ICompanyMongoRepository>(sp =>
{
    var mongoDb = sp.GetRequiredService<IMongoDatabase>();
    var collection = mongoDb.GetCollection<Manager.Core.Entities.Company>("companies");
    return new CompanyRepository(collection);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add SignalR for real-time chat
builder.Services.AddSignalR();

// Add Health Checks
builder.Services.AddCustomHealthChecks();

// Add Rate Limiting
builder.Services.AddCustomRateLimiting();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AvilaDashboard",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AvilaUsers",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-here"))
        };
    });

builder.Services.AddAuthorization();

// Configure CORS
var corsPolicy = "LandingCors";
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
        ?? new[] { "http://localhost:3000", "http://localhost:5000", "https://localhost:5000" };
    
    // Default policy for internal APIs
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Public policy for Landing page
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ManagerDbContext>();
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

// Add custom middleware
app.UseRequestLogging();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoints
app.MapCustomHealthChecks();

app.MapControllers();

// Map SignalR hubs
app.MapHub<Manager.Api.Hubs.ChatHub>("/hubs/chat");

app.Run();
