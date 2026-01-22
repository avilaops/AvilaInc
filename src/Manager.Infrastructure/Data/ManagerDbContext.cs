using Microsoft.EntityFrameworkCore;
using Manager.Core.Entities.Identity;
using Manager.Core.Entities.Clients;
using Manager.Core.Entities.Projects;
using Manager.Core.Entities.Provisioning;
using Manager.Core.Entities.Deploy;
using Manager.Core.Entities.Orchestration;
using Manager.Core.Entities.Audit;

namespace Manager.Infrastructure.Data;

public class ManagerDbContext : DbContext
{
    public ManagerDbContext(DbContextOptions<ManagerDbContext> options) : base(options) { }

    // Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();

    // Clients
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<BillingProfile> BillingProfiles => Set<BillingProfile>();

    // Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectSpec> ProjectSpecs => Set<ProjectSpec>();

    // Provisioning
    public DbSet<ProviderAccount> ProviderAccounts => Set<ProviderAccount>();
    public DbSet<Secret> Secrets => Set<Secret>();
    public DbSet<Domain> Domains => Set<Domain>();

    // Deploy
    public DbSet<Deployment> Deployments => Set<Deployment>();

    // Orchestration
    public DbSet<Playbook> Playbooks => Set<Playbook>();
    public DbSet<PlaybookRun> PlaybookRuns => Set<PlaybookRun>();
    public DbSet<StepRun> StepRuns => Set<StepRun>();

    // Audit
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<WebhookEvent> WebhookEvents => Set<WebhookEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Identity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Sessions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.Sessions).HasForeignKey(e => e.UserId);
        });

        // Clients
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contacts");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Client).WithMany(c => c.Contacts).HasForeignKey(e => e.ClientId);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<BillingProfile>(entity =>
        {
            entity.ToTable("BillingProfiles");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Client).WithOne(c => c.BillingProfile).HasForeignKey<BillingProfile>(e => e.ClientId);
        });

        // Projects
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
        });

        modelBuilder.Entity<ProjectSpec>(entity =>
        {
            entity.ToTable("ProjectSpecs");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Project).WithOne(p => p.Spec).HasForeignKey<ProjectSpec>(e => e.ProjectId);
            entity.Property(e => e.Brand).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CtaJson).HasColumnType("jsonb");
            entity.Property(e => e.ThemeJson).HasColumnType("jsonb");
            entity.Property(e => e.ContentJson).HasColumnType("jsonb");
            entity.Property(e => e.IntegrationsJson).HasColumnType("jsonb");
            entity.Property(e => e.DeployJson).HasColumnType("jsonb");
        });

        // Provisioning
        modelBuilder.Entity<ProviderAccount>(entity =>
        {
            entity.ToTable("ProviderAccounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider).HasMaxLength(100).IsRequired();
            entity.Property(e => e.AccountName).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Secret>(entity =>
        {
            entity.ToTable("Secrets");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ProviderAccount).WithMany(p => p.Secrets).HasForeignKey(e => e.ProviderAccountId);
            entity.Property(e => e.Key).HasMaxLength(255).IsRequired();
            entity.Property(e => e.EncryptedValue).IsRequired();
        });

        modelBuilder.Entity<Domain>(entity =>
        {
            entity.ToTable("Domains");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DomainName).IsUnique();
            entity.Property(e => e.DomainName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.DnsRecords).HasColumnType("jsonb");
        });

        // Deploy
        modelBuilder.Entity<Deployment>(entity =>
        {
            entity.ToTable("Deployments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProjectId);
            entity.Property(e => e.Version).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Provider).IsRequired();
        });

        // Orchestration
        modelBuilder.Entity<Playbook>(entity =>
        {
            entity.ToTable("Playbooks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(100).IsRequired();
            entity.Property(e => e.StepsJson).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<PlaybookRun>(entity =>
        {
            entity.ToTable("PlaybookRuns");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Playbook).WithMany(p => p.Runs).HasForeignKey(e => e.PlaybookId);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.InputJson).HasColumnType("jsonb");
            entity.Property(e => e.OutputJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<StepRun>(entity =>
        {
            entity.ToTable("StepRuns");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.PlaybookRun).WithMany(p => p.StepRuns).HasForeignKey(e => e.PlaybookRunId);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.StepName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.InputJson).HasColumnType("jsonb");
            entity.Property(e => e.OutputJson).HasColumnType("jsonb");
        });

        // Audit
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("AuditEvents");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Metadata).HasColumnType("jsonb");
        });

        modelBuilder.Entity<WebhookEvent>(entity =>
        {
            entity.ToTable("WebhookEvents");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Source);
            entity.HasIndex(e => e.IsProcessed);
            entity.Property(e => e.Source).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EventType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PayloadJson).HasColumnType("jsonb").IsRequired();
        });
    }
}
