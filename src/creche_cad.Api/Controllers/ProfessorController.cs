using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace creche_cad.Controllers;
[ApiController, Route("api/professor")]
public class ProfessorController(CrecheDbContext db) : ControllerBase {
    private IQueryable<ProfessorDto> Query() => db.Professores.AsNoTracking().OrderBy(p => p.Nome).Select(p => new ProfessorDto {
        Id = p.Id, Nome = p.Nome, RG = p.RG, CPF = p.CPF, Endereco = p.Endereco, TelefonePrincipal = p.TelefonePrincipal,
        TelefoneCelular = p.TelefoneCelular, TelefoneSecundario = p.TelefoneSecundario, Titulo = p.Titulo,
        CarteiraTrabalho = p.CarteiraTrabalho, DataAdmissao = p.DataAdmissao, DataDemissao = p.DataDemissao
    });
    [HttpGet]
    public async Task<IActionResult> ObterProfessores(CancellationToken ct) => Ok(await Query().ToListAsync(ct));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterProfessor(Guid id, CancellationToken ct) {
        var professor = await Query().SingleOrDefaultAsync(p => p.Id == id, ct); return professor is null ? NotFound() : Ok(professor);
    }
    [HttpPost]
    public async Task<IActionResult> CriarProfessor(ProfessorInputModel input, CancellationToken ct) {
        var professor = new Professor { Id = Guid.NewGuid(), DataCriacao = DateTime.UtcNow }; Apply(professor, input);
        db.Professores.Add(professor); await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(ObterProfessor), new { id = professor.Id }, await Query().SingleAsync(p => p.Id == professor.Id, ct));
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarProfessor(Guid id, ProfessorInputModel input, CancellationToken ct) {
        var professor = await db.Professores.FindAsync([id], ct); if (professor is null) return NotFound();
        Apply(professor, input); professor.DataAtualizacao = DateTime.UtcNow; await db.SaveChangesAsync(ct);
        return Ok(new { message = "Professor atualizado." });
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarProfessor(Guid id, CancellationToken ct) {
        var professor = await db.Professores.FindAsync([id], ct); if (professor is null) return NotFound();
        db.Professores.Remove(professor); await db.SaveChangesAsync(ct); return Ok(new { message = "Professor excluído." });
    }
    private static void Apply(Professor p, ProfessorInputModel i) {
        p.Nome = i.Nome.Trim(); p.RG = i.RG.Trim(); p.CPF = i.CPF.Trim(); p.Endereco = i.Endereco.Trim();
        p.TelefonePrincipal = i.TelefonePrincipal.Trim(); p.TelefoneCelular = i.TelefoneCelular?.Trim();
        p.TelefoneSecundario = i.TelefoneSecundario?.Trim(); p.Titulo = i.Titulo?.Trim(); p.CarteiraTrabalho = i.CarteiraTrabalho?.Trim();
        p.DataAdmissao = i.DataAdmissao; p.DataDemissao = i.DataDemissao;
    }
}
