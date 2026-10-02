using System.ComponentModel.DataAnnotations;

namespace creche_cad.Domain.Models
{
    public class AlunoInputModel : IValidatableObject
    {
        [Required(ErrorMessage = "O nome do aluno é obrigatório")]
        [StringLength(200)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A turma é obrigatória")]
        public Guid TurmaId { get; set; }

        [Required(ErrorMessage = "A data de nascimento do aluno é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O nome do pai do aluno é obrigatório")]
        [StringLength(200)]
        public string NomePai { get; set; }

        [Required(ErrorMessage = "O nome da mãe do aluno é obrigatório")]
        [StringLength(200)]
        public string NomeMae { get; set; }

        [Required(ErrorMessage = "O endereço do aluno é obrigatório")]
        [StringLength(200)]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Pelo menos um número de telefone é obrigatório")]
        [StringLength(200)]
        public string Telefone { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext context) {
            if (TurmaId == Guid.Empty) yield return new ValidationResult("Informe uma turma.", [nameof(TurmaId)]);
            if (DataNascimento == default || DataNascimento.Date > DateTime.UtcNow.Date)
                yield return new ValidationResult("Informe uma data de nascimento válida.", [nameof(DataNascimento)]);
        }
    }
}
