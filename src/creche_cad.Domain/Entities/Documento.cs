namespace creche_cad.Domain.Entities
{
    public class Documento
    {
        public Guid Id { get; set; }
        public Guid? AlunoId { get; set; }
        public Guid? ProfessorId { get; set; }
        public string NomeArquivo { get; set; }
        public byte[] DocumentoBytes { get; set; }
        public DateTime DataCriacao { get; set; }
        public virtual Aluno Aluno { get; set; }
        public virtual Professor Professor { get; set; }
    }
}