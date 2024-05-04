namespace creche_cad.Domain.Entities
{
    public class Aluno
    {
        public Guid Id { get; set; }
        public Guid TurmaId { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public virtual Turma Turma { get; set; }
        public virtual ICollection<Documento> Documentos { get; set; }
    }
}