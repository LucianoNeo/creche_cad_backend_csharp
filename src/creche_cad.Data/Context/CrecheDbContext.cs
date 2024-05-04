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

        public DbSet<Turma> Turmas { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<Documento> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new TurmaConfiguracao());
            builder.ApplyConfiguration(new AlunoConfiguracao());
            builder.ApplyConfiguration(new ProfessorConfiguracao());
            builder.ApplyConfiguration(new DocumentoConfiguracao());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}