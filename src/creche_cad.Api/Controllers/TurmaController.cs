using creche_cad.Data.Context;
using creche_cad.Domain.Dtos;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace creche_cad.Controllers;
[ApiController, Route("api/turma")]
public class TurmaController(CrecheDbContext db) : ControllerBase {
    private IQueryable<TurmaDto> Query() => db.Turmas.AsNoTracking().OrderBy(t => t.Nome).Select(t => new TurmaDto { Id = t.Id, Nome = t.Nome, Metragem = t.Metragem });
    [HttpGet]
    public async Task<IActionResult> ObterTurmas(CancellationToken ct,int page=1,int pageSize=10,string? q=null) {
        var query=Query();
        if(!string.IsNullOrWhiteSpace(q))query=query.Where(x=>x.Nome.Contains(q));
        var size=Math.Clamp(pageSize,1,100); var current=Math.Max(1,page);
        return Ok(new {items=await query.Skip((current-1)*size).Take(size).ToListAsync(ct),total=await query.CountAsync(ct),page=current,pageSize=size});
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterTurma(Guid id, CancellationToken ct) {
        var turma = await Query().SingleOrDefaultAsync(t => t.Id == id, ct); return turma is null ? NotFound() : Ok(turma);
    }
    [HttpPost]
    public async Task<IActionResult> CriarTurma(TurmaInputModel input, CancellationToken ct) {
        var turma = new Turma { Id = Guid.NewGuid(), Nome = input.Nome.Trim(), Metragem = input.Metragem?.Trim(), DataCriacao = DateTime.UtcNow };
        db.Turmas.Add(turma); await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(ObterTurma), new { id = turma.Id }, new TurmaDto { Id = turma.Id, Nome = turma.Nome, Metragem = turma.Metragem });
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarTurma(Guid id, TurmaInputModel input, CancellationToken ct) {
        var turma = await db.Turmas.FindAsync([id], ct); if (turma is null) return NotFound();
        turma.Nome = input.Nome.Trim(); turma.Metragem = input.Metragem?.Trim(); turma.DataAtualizacao = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); return Ok(new { message = "Turma atualizada." });
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarTurma(Guid id, CancellationToken ct) {
        var turma = await db.Turmas.FindAsync([id], ct); if (turma is null) return NotFound();
        if (await db.Alunos.AnyAsync(a => a.TurmaId == id, ct)) return Conflict(new { message = "Transfira os alunos antes de excluir esta turma." });
        db.Turmas.Remove(turma); await db.SaveChangesAsync(ct); return Ok(new { message = "Turma excluída." });
    }
}
