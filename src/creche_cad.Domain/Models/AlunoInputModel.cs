using System.ComponentModel.DataAnnotations;

namespace creche_cad.Domain.Models
{
    public class AlunoInputModel
    {
        [Required(ErrorMessage = "O nome do aluno é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A turma é obrigatória")]
        public Guid TurmaId { get; set; }

        [Required(ErrorMessage = "A data de nascimento do aluno é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O nome do pai do aluno é obrigatório")]
        public string NomePai { get; set; }

        [Required(ErrorMessage = "O nome da mãe do aluno é obrigatório")]
        public string NomeMae { get; set; }

        [Required(ErrorMessage = "O endereço do aluno é obrigatório")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Pelo menos um número de telefone é obrigatório")]
        public string Telefone { get; set; }
    }
}