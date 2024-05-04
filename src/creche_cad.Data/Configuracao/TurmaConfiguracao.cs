using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace creche_cad.Data.Configuracao
{
    public class TurmaConfiguracao : IEntityTypeConfiguration<Turma>
    {
        public void Configure(EntityTypeBuilder<Turma> builder)
        {
            builder.ToTable(nameof(Turma));
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nome).IsRequired();
            builder.Property(t => t.Metragem);
            builder.Property(t => t.DataCriacao);
            builder.Property(t => t.DataAtualizacao);
            builder.Property(t => t.DataExclusao);

            builder.HasMany(t => t.Alunos)
                   .WithOne()
                   .HasForeignKey(a => a.TurmaId);
        }
    }
}