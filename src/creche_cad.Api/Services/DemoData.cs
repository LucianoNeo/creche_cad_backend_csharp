using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace creche_cad.Api.Services;
public static class DemoData {
    public static async Task SeedAsync(CrecheDbContext db) {
        if (await db.Turmas.AnyAsync()) return;
        var names = new[] { "Maternal · Girassol", "Jardim · Ipê", "Pré-escola · Manacá" };
        var turmas = names.Select(n => new Turma { Id = Guid.NewGuid(), Nome = n, Metragem = "42 m²", DataCriacao = DateTime.UtcNow, Alunos = new List<Aluno>() }).ToArray();
        db.Turmas.AddRange(turmas);
        var students = new[] { "Alice Martins", "Bernardo Lima", "Clara Oliveira", "Davi Santos", "Elisa Costa", "Felipe Rocha", "Helena Alves", "Igor Pereira", "Laura Ribeiro", "Miguel Souza", "Nina Castro", "Theo Ramos" };
        for (var i = 0; i < students.Length; i++) db.Alunos.Add(new Aluno {
            Id = Guid.NewGuid(), Nome = students[i], TurmaId = turmas[i % 3].Id, Turma = turmas[i % 3],
            DataNascimento = new DateTime(2021 + i % 3, 2 + i % 8, 12), NomePai = "Responsável A (fictício)", NomeMae = "Responsável B (fictício)",
            Endereco = "Rua de demonstração, 100", Telefone = "(00) 00000-0000", DataCriacao = DateTime.UtcNow
        });
        foreach (var name in new[] { "Ana Ferreira", "Mariana Lopes", "Rafael Mendes" }) db.Professores.Add(new Professor {
            Id = Guid.NewGuid(), Nome = name, RG = "DEMO", CPF = "000.000.000-00", Endereco = "Rua de demonstração, 100",
            TelefonePrincipal = "(00) 00000-0000", TelefoneCelular = "(00) 00000-0001", Titulo = "Pedagogia",
            CarteiraTrabalho = "DEMO", DataAdmissao = new DateTime(2024, 2, 1), DataCriacao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
