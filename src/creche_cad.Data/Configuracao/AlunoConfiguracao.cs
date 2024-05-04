using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace creche_cad.Data.Configuracao
{
    public class AlunoConfiguracao : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.ToTable(nameof(Aluno));
            builder.HasKey(t => t.Id);

            builder.Property(a => a.Nome).IsRequired();
            builder.Property(a => a.DataNascimento).IsRequired();
            builder.Property(a => a.NomePai);
            builder.Property(a => a.NomeMae);
            builder.Property(a => a.Endereco);
            builder.Property(a => a.Telefone);
            builder.Property(a => a.DataCriacao).IsRequired();
            builder.Property(a => a.DataAtualizacao);

            builder.HasOne(a => a.Turma)
                   .WithMany(t => t.Alunos)
                   .HasForeignKey(a => a.TurmaId)
                   .IsRequired();

            builder.HasMany(a => a.Documentos)
                   .WithOne()
                   .HasForeignKey(d => d.AlunoId);
        }
    }
}