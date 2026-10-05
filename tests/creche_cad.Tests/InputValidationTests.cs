using creche_cad.Domain.Models;
using creche_cad.Service.Validation;

namespace creche_cad.Tests;

public sealed class InputValidationTests
{
    [Fact]
    public async Task Student_requires_a_name_class_and_non_future_birth_date()
    {
        var model = new AlunoInputModel { DataNascimento = DateTime.UtcNow.Date.AddDays(1) };
        var result = await new AlunoInputModelValidator().ValidateAsync(model);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(model.Nome));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(model.TurmaId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(model.DataNascimento));
    }

    [Fact]
    public async Task Class_rejects_blank_or_overlong_names()
    {
        var validator = new TurmaInputModelValidator();

        var blank = await validator.ValidateAsync(new TurmaInputModel { Nome = "   " });
        var tooLong = await validator.ValidateAsync(new TurmaInputModel { Nome = new string('A', 101) });

        Assert.False(blank.IsValid);
        Assert.False(tooLong.IsValid);
    }

    [Fact]
    public async Task Teacher_cannot_be_dismissed_before_hiring()
    {
        var hired = new DateTime(2024, 4, 1);
        var result = await new ProfessorInputModelValidator().ValidateAsync(new ProfessorInputModel
        {
            Nome = "Lucia",
            RG = "RG",
            CPF = "CPF",
            Endereco = "Rua A",
            TelefonePrincipal = "11999999999",
            DataAdmissao = hired,
            DataDemissao = hired.AddDays(-1)
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProfessorInputModel.DataDemissao));
    }
}
