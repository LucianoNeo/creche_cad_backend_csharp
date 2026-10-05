namespace creche_cad.Domain.Models;

public sealed class AlunoInputModel
{
    public string Nome { get; set; } = "";
    public Guid TurmaId { get; set; }
    public DateTime DataNascimento { get; set; }
    public string NomePai { get; set; } = "";
    public string NomeMae { get; set; } = "";
    public string Endereco { get; set; } = "";
    public string Telefone { get; set; } = "";
}
