using MongoDB.Driver;
using Manager.Infrastructure.Data;

var settings = new MongoDbSettings
{
    ConnectionString = "mongodb+srv://nicolasrosaab_db_user:Gio4EAQhbEdQMISl@cluster0.npuhras.mongodb.net/",
    DatabaseNames = new DatabaseNames
    {
        Dashboard = "avila_dashboard",
        Gmail = "avila_gmail",
        Crm = "avila_crm"
    }
};

var context = new MongoDbContext(settings.ConnectionString);

// Listar coleções do CRM
Console.WriteLine("=== Coleções em avila_crm ===");
var crmDb = context.GetDatabase(settings.DatabaseNames.Crm);
var crmCollections = await crmDb.ListCollectionNamesAsync();
await crmCollections.ForEachAsync(name => Console.WriteLine($"- {name}"));

Console.WriteLine("\n=== Coleções em avila_gmail ===");
var gmailDb = context.GetDatabase(settings.DatabaseNames.Gmail);
var gmailCollections = await gmailDb.ListCollectionNamesAsync();
await gmailCollections.ForEachAsync(name => Console.WriteLine($"- {name}"));

Console.WriteLine("\n=== Coleções em avila_dashboard ===");
var dashboardDb = context.GetDatabase(settings.DatabaseNames.Dashboard);
var dashboardCollections = await dashboardDb.ListCollectionNamesAsync();
await dashboardCollections.ForEachAsync(name => Console.WriteLine($"- {name}"));

// Contar documentos em leads
var leadsCollection = context.GetCollection<MongoDB.Bson.BsonDocument>(settings.DatabaseNames.Crm, "leads");
var leadsCount = await leadsCollection.CountDocumentsAsync(MongoDB.Bson.BsonDocument.Parse("{}"));
Console.WriteLine($"\n=== Total de Leads: {leadsCount} ===");

// Contar documentos em contacts
var contactsCollection = context.GetCollection<MongoDB.Bson.BsonDocument>(settings.DatabaseNames.Crm, "contacts");
var contactsCount = await contactsCollection.CountDocumentsAsync(MongoDB.Bson.BsonDocument.Parse("{}"));
Console.WriteLine($"=== Total de Contacts: {contactsCount} ===");

// Contar documentos em campanhas
var campanhasCollection = context.GetCollection<MongoDB.Bson.BsonDocument>(settings.DatabaseNames.Crm, "campanhas");
var campanhasCount = await campanhasCollection.CountDocumentsAsync(MongoDB.Bson.BsonDocument.Parse("{}"));
Console.WriteLine($"=== Total de Campanhas: {campanhasCount} ===");

Console.WriteLine("\n✅ Verificação concluída!");
