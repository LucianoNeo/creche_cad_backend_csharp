using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
namespace creche_cad.Controllers;
[ApiController, Route("api/documento")]
public class DocumentoController(CrecheDbContext db) : ControllerBase {
    private const long MaximumFileBytes = 5 * 1024 * 1024;
    private IQueryable<Documento> Documents(Guid id, bool aluno) => db.Documentos.Where(d => aluno ? d.AlunoId == id : d.ProfessorId == id);
    private Task<bool> Exists(Guid id, bool aluno, CancellationToken ct) => aluno ? db.Alunos.AnyAsync(a => a.Id == id, ct) : db.Professores.AnyAsync(p => p.Id == id, ct);
    [HttpPost("aluno/{id:guid}/upload"), RequestSizeLimit(16 * 1024 * 1024)]
    public Task<IActionResult> UploadAluno(Guid id, [FromForm] List<IFormFile> files, CancellationToken ct) => Upload(id, true, files, ct);
    [HttpPost("professor/{id:guid}/upload"), RequestSizeLimit(16 * 1024 * 1024)]
    public Task<IActionResult> UploadProfessor(Guid id, [FromForm] List<IFormFile> files, CancellationToken ct) => Upload(id, false, files, ct);
    private async Task<IActionResult> Upload(Guid id, bool aluno, List<IFormFile> files, CancellationToken ct) {
        if (!await Exists(id, aluno, ct)) return NotFound();
        if (files.Count is < 1 or > 3) return BadRequest(new { message = "Envie de um a três arquivos por vez." });
        var pending = new List<Documento>();
        foreach (var file in files) {
            var name = Path.GetFileName(file.FileName.Replace('\\', '/'));
            if (file.Length is <= 0 or > MaximumFileBytes || name.Length is < 1 or > 150)
                return BadRequest(new { message = "Cada arquivo deve ter até 5 MB e um nome de até 150 caracteres." });
            using var stream = new MemoryStream(); await file.CopyToAsync(stream, ct);
            var bytes = stream.ToArray(); var extension = Path.GetExtension(name).ToLowerInvariant();
            var valid = extension switch {
                ".pdf" => bytes.AsSpan().StartsWith("%PDF-"u8),
                ".png" => bytes.AsSpan().StartsWith(new byte[] { 137,80,78,71,13,10,26,10 }),
                ".jpg" or ".jpeg" => bytes.AsSpan().StartsWith(new byte[] { 255,216,255 }),
                ".txt" => !bytes.Contains((byte)0), _ => false
            };
            if (!valid) return BadRequest(new { message = "Use PDF, PNG, JPG ou TXT com conteúdo correspondente ao formato." });
            pending.Add(new Documento { Id = Guid.NewGuid(), AlunoId = aluno ? id : null, ProfessorId = aluno ? null : id,
                NomeArquivo = name, DocumentoBytes = bytes, DataCriacao = DateTime.UtcNow });
        }
        db.Documentos.AddRange(pending); await db.SaveChangesAsync(ct);
        return Ok(new { message = "Documentos enviados." });
    }
    [HttpGet("aluno/{id:guid}/documentos")]
    public Task<IActionResult> ListAluno(Guid id, CancellationToken ct) => List(id, true, ct);
    [HttpGet("professor/{id:guid}/documentos")]
    public Task<IActionResult> ListProfessor(Guid id, CancellationToken ct) => List(id, false, ct);
    private async Task<IActionResult> List(Guid id, bool aluno, CancellationToken ct) {
        if (!await Exists(id, aluno, ct)) return NotFound();
        return Ok(await Documents(id, aluno).AsNoTracking().Select(d => new { d.Id, d.NomeArquivo }).ToListAsync(ct));
    }
    [HttpGet("aluno/{id:guid}/download")]
    public Task<IActionResult> ZipAluno(Guid id, CancellationToken ct) => Zip(id, true, ct);
    [HttpGet("professor/{id:guid}/download")]
    public Task<IActionResult> ZipProfessor(Guid id, CancellationToken ct) => Zip(id, false, ct);
    private async Task<IActionResult> Zip(Guid id, bool aluno, CancellationToken ct) {
        if (!await Exists(id, aluno, ct)) return NotFound();
        var documents = await Documents(id, aluno).AsNoTracking().ToListAsync(ct);
        if (documents.Count == 0) return NotFound();
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true)) {
            foreach (var document in documents) {
                // IDs avoid collisions when multiple uploads have the same filename.
                var entry = archive.CreateEntry($"{document.Id:N}-{document.NomeArquivo}");
                await using var output = entry.Open(); await output.WriteAsync(document.DocumentoBytes, ct);
            }
        }
        return File(stream.ToArray(), "application/zip", $"documentos-{id:N}.zip");
    }
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct) {
        var d = await db.Documentos.AsNoTracking().SingleOrDefaultAsync(d => d.Id == id, ct);
        return d is null ? NotFound() : File(d.DocumentoBytes, "application/octet-stream", d.NomeArquivo);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) {
        var d = await db.Documentos.FindAsync([id], ct); if (d is null) return NotFound();
        db.Documentos.Remove(d); await db.SaveChangesAsync(ct); return NoContent();
    }
    [HttpDelete("aluno/{id:guid}/documentos")]
    public Task<IActionResult> DeleteAluno(Guid id, CancellationToken ct) => DeleteAll(id, true, ct);
    [HttpDelete("professor/{id:guid}/documentos")]
    public Task<IActionResult> DeleteProfessor(Guid id, CancellationToken ct) => DeleteAll(id, false, ct);
    private async Task<IActionResult> DeleteAll(Guid id, bool aluno, CancellationToken ct) {
        if (!await Exists(id, aluno, ct)) return NotFound();
        await Documents(id, aluno).ExecuteDeleteAsync(ct); return NoContent();
    }
}
