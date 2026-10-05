using creche_cad.Domain.Models;
using FluentValidation;

namespace creche_cad.Service.Validation;

public sealed class AlunoInputModelValidator : AbstractValidator<AlunoInputModel>
{
    public AlunoInputModelValidator()
    {
        RuleFor(model => model.Nome).NotEmpty().MaximumLength(200);
        RuleFor(model => model.TurmaId).NotEmpty();
        RuleFor(model => model.DataNascimento).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow.Date);
        RuleFor(model => model.NomePai).NotEmpty().MaximumLength(200);
        RuleFor(model => model.NomeMae).NotEmpty().MaximumLength(200);
        RuleFor(model => model.Endereco).NotEmpty().MaximumLength(200);
        RuleFor(model => model.Telefone).NotEmpty().MaximumLength(200);
    }
}

public sealed class ProfessorInputModelValidator : AbstractValidator<ProfessorInputModel>
{
    public ProfessorInputModelValidator()
    {
        RuleFor(model => model.Nome).NotEmpty().MaximumLength(200);
        RuleFor(model => model.RG).NotEmpty().MaximumLength(200);
        RuleFor(model => model.CPF).NotEmpty().MaximumLength(200);
        RuleFor(model => model.Endereco).NotEmpty().MaximumLength(200);
        RuleFor(model => model.TelefonePrincipal).NotEmpty().MaximumLength(200);
        RuleFor(model => model.TelefoneCelular).MaximumLength(200);
        RuleFor(model => model.TelefoneSecundario).MaximumLength(200);
        RuleFor(model => model.Titulo).MaximumLength(200);
        RuleFor(model => model.CarteiraTrabalho).MaximumLength(200);
        RuleFor(model => model.DataAdmissao).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow.Date);
        RuleFor(model => model.DataDemissao)
            .Must((model, terminationDate) => !terminationDate.HasValue || terminationDate.Value.Date >= model.DataAdmissao.Date)
            .WithMessage("A demissão não pode ser anterior à admissão.");
    }
}

public sealed class TurmaInputModelValidator : AbstractValidator<TurmaInputModel>
{
    public TurmaInputModelValidator()
    {
        RuleFor(model => model.Nome).NotEmpty().MaximumLength(100);
        RuleFor(model => model.Metragem).MaximumLength(50);
    }
}
