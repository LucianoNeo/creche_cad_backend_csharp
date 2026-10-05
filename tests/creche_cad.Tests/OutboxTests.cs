using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Tests;

public sealed class OutboxTests
{
    [Fact]
    public async Task Student_and_class_changes_publish_only_a_data_free_dashboard_event()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<CrecheDbContext>().UseSqlite(connection).Options;
        await using var db = new CrecheDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var classroom = new Turma { Id = Guid.NewGuid(), Nome = "Sala Azul", DataCriacao = DateTime.UtcNow };
        db.Turmas.Add(classroom);
        await db.SaveChangesAsync();

        var student = new Aluno
        {
            Id = Guid.NewGuid(),
            TurmaId = classroom.Id,
            Nome = "Pessoa Fictícia",
            DataNascimento = new DateTime(2020, 1, 1),
            NomePai = "Responsável Fictício",
            NomeMae = "Responsável Fictícia",
            Endereco = "Endereço Fictício",
            Telefone = "0000000000",
            DataCriacao = DateTime.UtcNow
        };
        db.Alunos.Add(student);
        await db.SaveChangesAsync();

        var messages = await db.Outbox.AsNoTracking().ToListAsync();
        Assert.Equal(2, messages.Count);
        Assert.All(messages, message => Assert.Equal("school.summary.invalidate.v1", message.EventType));
        Assert.All(messages, message => Assert.Equal("{}", message.Payload));
        Assert.DoesNotContain("Pessoa Fictícia", string.Join(' ', messages.Select(message => message.Payload)));

        student.Nome = "Nome Alterado";
        await db.SaveChangesAsync();
        Assert.Equal(2, await db.Outbox.CountAsync());
    }
}
