namespace Manager.Infrastructure.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseNames DatabaseNames { get; set; } = new();
}

public class DatabaseNames
{
    public string Dashboard { get; set; } = "avila_dashboard";
    public string Gmail { get; set; } = "avila_gmail";
    public string Crm { get; set; } = "avila_crm";
}
