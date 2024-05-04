using System.ComponentModel.DataAnnotations;

namespace creche_cad.Domain.Dtos
{
    public class AlunoDto
    {
        public Guid Id { get; set; }
        public Guid TurmaId { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
    }
}