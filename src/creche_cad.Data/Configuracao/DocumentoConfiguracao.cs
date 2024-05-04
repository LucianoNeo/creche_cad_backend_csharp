using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace creche_cad.Data.Configuracao
{
    public class DocumentoConfiguracao : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            builder.ToTable(nameof(Documento));
            builder.HasKey(d => d.Id);

            builder.Property(d => d.NomeArquivo).IsRequired();
            builder.Property(d => d.DocumentoBytes).IsRequired();
            builder.Property(d => d.DataCriacao);

            builder.HasOne(d => d.Aluno)
                   .WithMany(a => a.Documentos)
                   .HasForeignKey(d => d.AlunoId);

            builder.HasOne(d => d.Professor)
                   .WithMany(p => p.Documentos)
                   .HasForeignKey(d => d.ProfessorId);
        }
    }
}
