using creche_cad.Data.Configuracao;
using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Data.Context
{
    public class CrecheDbContext : DbContext
    {
        public CrecheDbContext()
        { }
        public CrecheDbContext(DbContextOptions<CrecheDbContext> options) : base(options)
        {
        }

        public DbSet<SchoolUser> Users => Set<SchoolUser>();
        public DbSet<AuditEntry> Audit => Set<AuditEntry>();
        public DbSet<AccessRecovery> Recoveries => Set<AccessRecovery>();
        public string AuditActor { get; set; } = "system";
        public DbSet<Turma> Turmas { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SchoolUser>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<SchoolUser>().Property(u => u.Username).HasMaxLength(100);
            builder.Entity<AuditEntry>().HasIndex(a => a.At);
            builder.Entity<OutboxMessage>().HasIndex(message => new { message.PublishedAtUtc, message.OccurredAtUtc });

            builder.ApplyConfiguration(new TurmaConfiguracao());
            builder.ApplyConfiguration(new AlunoConfiguracao());
            builder.ApplyConfiguration(new ProfessorConfiguracao());
            builder.ApplyConfiguration(new DocumentoConfiguracao());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            var changes = ChangeTracker.Entries().Where(e => e.Entity is not AuditEntry and not OutboxMessage && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).ToArray();
            var events = changes
                .Select(e => new AuditEntry { Actor = AuditActor, Action = e.State.ToString(), Resource = e.Metadata.ClrType.Name,
                    ResourceId = e.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "" }).ToArray();
            Audit.AddRange(events);
            var affectsDashboard = changes.Any(entry => entry.Entity switch
            {
                Turma => true,
                Aluno when entry.State is EntityState.Added or EntityState.Deleted => true,
                Aluno => entry.Property(nameof(Aluno.TurmaId)).IsModified,
                Professor => entry.State is EntityState.Added or EntityState.Deleted,
                _ => false
            });
            if (affectsDashboard)
                Outbox.Add(new OutboxMessage { EventType = "school.summary.invalidate.v1", Payload = "{}" });
            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Personal information must not be written to application logs.
        }
    }
}
