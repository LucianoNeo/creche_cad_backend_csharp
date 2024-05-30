namespace creche_cad.Domain.Entities
{
    public class Turma
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Metragem { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public ICollection<Aluno> Alunos { get; set; }
    }
}