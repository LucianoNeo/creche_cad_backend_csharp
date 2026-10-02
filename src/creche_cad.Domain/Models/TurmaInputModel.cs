using System.ComponentModel.DataAnnotations;
namespace creche_cad.Domain.Models;
public class TurmaInputModel {
    [Required, StringLength(100)] public string Nome { get; set; } = "";
    [StringLength(50)] public string? Metragem { get; set; }
}
