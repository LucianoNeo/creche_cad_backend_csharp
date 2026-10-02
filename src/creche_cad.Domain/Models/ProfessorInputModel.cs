using System.ComponentModel.DataAnnotations;

namespace creche_cad.Domain.Models
{
    public class ProfessorInputModel : IValidatableObject
    {
        [Required(ErrorMessage = "O nome do professor é obrigatório")]
        [StringLength(200)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O RG do professor é obrigatório")]
        [StringLength(200)]
        public string RG { get; set; }

        [Required(ErrorMessage = "O CPF do professor é obrigatório")]
        [StringLength(200)]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O endereço do professor é obrigatório")]
        [StringLength(200)]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Pelo menos um número de telefone é obrigatório")]
        [StringLength(200)]
        public string TelefonePrincipal { get; set; }

        [StringLength(200)]
        public string? TelefoneCelular { get; set; }

        [StringLength(200)]
        public string? TelefoneSecundario { get; set; }

        [StringLength(200)]
        public string? Titulo { get; set; }

        [StringLength(200)]
        public string? CarteiraTrabalho { get; set; }

        [Required(ErrorMessage = "A data de admissão do professor é obrigatória")]
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext context) {
            if (DataAdmissao == default || DataAdmissao.Date > DateTime.UtcNow.Date)
                yield return new ValidationResult("Informe uma data de admissão válida.", [nameof(DataAdmissao)]);
            if (DataDemissao.HasValue && DataDemissao.Value.Date < DataAdmissao.Date)
                yield return new ValidationResult("A demissão não pode ser anterior à admissão.", [nameof(DataDemissao)]);
        }
    }
}
