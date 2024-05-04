using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace creche_cad.Data.Configuracao
{
    public class ProfessorConfiguracao : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.ToTable(nameof(Professor));
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome).IsRequired();
            builder.Property(p => p.RG).IsRequired();
            builder.Property(p => p.CPF).IsRequired();
            builder.Property(p => p.Endereco);
            builder.Property(p => p.TelefonePrincipal).IsRequired();
            builder.Property(p => p.TelefoneCelular);
            builder.Property(p => p.TelefoneSecundario);
            builder.Property(p => p.Titulo);
            builder.Property(p => p.CarteiraTrabalho);
            builder.Property(p => p.DataAdmissao).IsRequired();
            builder.Property(p => p.DataDemissao);
            builder.Property(p => p.DataCriacao);
            builder.Property(p => p.DataAtualizacao);

            builder.HasMany(p => p.Documentos)
                   .WithOne()
                   .HasForeignKey(d => d.ProfessorId);
        }
    }
}
