using creche_cad.Data.Context;
using creche_cad.Domain.Dtos;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace creche_cad.Controllers;
[Route("api/[controller]"), ApiController]
public class AlunoController(CrecheDbContext db) : ControllerBase {
    private static readonly Expression<Func<Aluno, AlunoDto>> Projection = a => new AlunoDto {
        Id = a.Id, Nome = a.Nome, DataNascimento = a.DataNascimento, NomePai = a.NomePai, NomeMae = a.NomeMae,
        Endereco = a.Endereco, Telefone = a.Telefone, TurmaId = a.TurmaId, TurmaNome = a.Turma.Nome
    };
    [HttpGet]
    public async Task<IActionResult> ObterAlunos(CancellationToken ct) => Ok(await db.Alunos.AsNoTracking().OrderBy(a => a.Nome).Select(Projection).ToListAsync(ct));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterAluno(Guid id, CancellationToken ct) {
        var aluno = await db.Alunos.AsNoTracking().Where(a => a.Id == id).Select(Projection).SingleOrDefaultAsync(ct);
        return aluno is null ? NotFound() : Ok(aluno);
    }
    [HttpPost]
    public async Task<IActionResult> CriarAluno(AlunoInputModel input, CancellationToken ct) {
        if (!await db.Turmas.AnyAsync(t => t.Id == input.TurmaId, ct)) return BadRequest(new { message = "A turma informada não existe." });
        var aluno = new Aluno { Id = Guid.NewGuid(), DataCriacao = DateTime.UtcNow };
        Apply(aluno, input);
        db.Alunos.Add(aluno);
        await db.SaveChangesAsync(ct);
        var dto = await db.Alunos.Where(a => a.Id == aluno.Id).Select(Projection).SingleAsync(ct);
        return CreatedAtAction(nameof(ObterAluno), new { id = aluno.Id }, dto);
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarAluno(Guid id, AlunoInputModel input, CancellationToken ct) {
        var aluno = await db.Alunos.FindAsync([id], ct);
        if (aluno is null) return NotFound();
        if (!await db.Turmas.AnyAsync(t => t.Id == input.TurmaId, ct)) return BadRequest(new { message = "A turma informada não existe." });
        Apply(aluno, input); aluno.DataAtualizacao = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Ok(new { message = "Aluno atualizado." });
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarAluno(Guid id, CancellationToken ct) {
        var aluno = await db.Alunos.FindAsync([id], ct);
        if (aluno is null) return NotFound();
        db.Alunos.Remove(aluno); await db.SaveChangesAsync(ct); return NoContent();
    }
    private static void Apply(Aluno a, AlunoInputModel i) {
        a.Nome = i.Nome.Trim(); a.TurmaId = i.TurmaId; a.DataNascimento = i.DataNascimento;
        a.NomePai = i.NomePai.Trim(); a.NomeMae = i.NomeMae.Trim(); a.Endereco = i.Endereco.Trim(); a.Telefone = i.Telefone.Trim();
    }
}
