var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register custom services
builder.Services.AddScoped<Landing.Services.ScrollService>();
builder.Services.AddScoped<Landing.Services.TrackingContext>();

// Configure HttpClient for Manager.Api integration
builder.Services.AddHttpClient<Landing.Services.ManagerApiClient>(client =>
{
    var baseUrl = builder.Configuration["ManagerApi:BaseUrl"]
        ?? throw new InvalidOperationException("ManagerApi:BaseUrl não configurado");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("User-Agent", "Landing/1.0");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Remover HTTPS redirect para produção no Render
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
