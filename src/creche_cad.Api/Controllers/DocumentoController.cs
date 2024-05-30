using creche_cad.Data.Context;
using creche_cad.Domain.Dtos;
using creche_cad.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly CrecheDbContext _context;

        public DocumentoController(CrecheDbContext context)
        {
            _context = context;
        }

        [HttpPost("aluno/{id}/upload")]
        public IActionResult UploadDocumentosAluno(Guid id, List<IFormFile> files)
        {
            var documentos = files;

            if (documentos == null || documentos.Count == 0)
            {
                return BadRequest("Nenhum arquivo enviado");
            }

            foreach (var documento in documentos)
            {
                using (var ms = new MemoryStream())
                {
                    documento.CopyTo(ms);
                    var documentoBytes = ms.ToArray();

                    var documentoEntity = new Documento
                    {
                        AlunoId = id,
                        NomeArquivo = documento.FileName,
                        DocumentoBytes = documentoBytes,
                        DataCriacao = DateTime.Now
                    };

                    _context.Documentos.Add(documentoEntity);
                }
            }

            _context.SaveChanges();

            return Ok(new { message = "Arquivos enviados com sucesso" });
        }

        [HttpDelete("aluno/{id}/documentos")]
        public IActionResult DeletarDocumentosAluno(Guid id)
        {
            var documentos = _context.Documentos.Where(d => d.AlunoId == id);
            _context.Documentos.RemoveRange(documentos);
            _context.SaveChanges();
            return Ok(new { message = "Documentos deletados com sucesso" });
        }

        [HttpPost("professor/{id}/upload")]
        public IActionResult UploadDocumentosProfessor(Guid id, List<IFormFile> files)
        {
            var documentos = files;

            if (documentos == null || documentos.Count == 0)
            {
                return BadRequest("Nenhum arquivo enviado");
            }

            foreach (var documento in documentos)
            {
                using (var ms = new MemoryStream())
                {
                    documento.CopyTo(ms);
                    var documentoBytes = ms.ToArray();

                    var documentoEntity = new Documento
                    {
                        ProfessorId = id,
                        DocumentoBytes = documentoBytes,
                        DataCriacao = DateTime.Now,
                        NomeArquivo = documento.FileName
                    };

                    _context.Documentos.Add(documentoEntity);
                }
            }

            _context.SaveChanges();

            return Ok(new { message = "Arquivos enviados com sucesso" });
        }

        [HttpDelete("professor/{id}/documentos")]
        public IActionResult DeletarDocumentosProfessor(Guid id)
        {
            var documentos = _context.Documentos.Where(d => d.ProfessorId == id);
            _context.Documentos.RemoveRange(documentos);
            _context.SaveChanges();
            return Ok(new { message = "Documentos deletados com sucesso" });
        }

        [HttpGet("aluno/{id}/documentos")]
        public IActionResult ObterDocumentosAluno(Guid id)
        {
            var documentos = _context.Documentos
                .Where(d => d.AlunoId == id)
                .Select(d => new DocumentoDto { Id = d.Id, NomeArquivo = d.NomeArquivo })
                .ToList();
            return Ok(documentos);
        }

        [HttpGet("professor/{id}/documentos")]
        public IActionResult ObterDocumentosProfessor(Guid id)
        {
            var documentos = _context.Documentos
                .Where(d => d.ProfessorId == id)
                .Select(d => new DocumentoDto { Id = d.Id, NomeArquivo = d.NomeArquivo })
                .ToList();
            return Ok(documentos);
        }

        [HttpGet("aluno/{id}/download")]
        public IActionResult DownloadDocumentosAluno(Guid id)
        {
            var nomeAluno = _context.Alunos.Where(a => a.Id == id).FirstOrDefault().Nome;

            nomeAluno = StringUtil.FormatNome(nomeAluno);

            var documentos = _context.Documentos.Where(d => d.AlunoId == id).ToList();

            if (documentos.Count == 0)
            {
                return NotFound("Nenhum documento encontrado para este aluno");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var documento in documentos)
                    {
                        var entry = archive.CreateEntry(documento.NomeArquivo);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(documento.DocumentoBytes, 0, documento.DocumentoBytes.Length);
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", $"documentos_{nomeAluno}.zip");
            }
        }

        [HttpGet("professor/{id}/download")]
        public IActionResult DownloadDocumentosProfessor(Guid id)
        {
            var nomeProfessor = _context.Professores.Where(a => a.Id == id).FirstOrDefault().Nome;

            nomeProfessor = StringUtil.FormatNome(nomeProfessor);

            var documentos = _context.Documentos.Where(d => d.ProfessorId == id).ToList();

            if (documentos.Count == 0)
            {
                return NotFound("Nenhum documento encontrado para este professor");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var documento in documentos)
                    {
                        var entry = archive.CreateEntry(documento.NomeArquivo);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(documento.DocumentoBytes, 0, documento.DocumentoBytes.Length);
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", $"documentos_{nomeProfessor}.zip");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarDocumento(Guid id)
        {
            var documento = _context.Documentos.Find(id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado");
            }

            _context.Documentos.Remove(documento);
            _context.SaveChanges();

            return Ok(new { message = "Documento deletado com sucesso" });
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadDocumento(Guid id)
        {
            var documento = _context.Documentos.Find(id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado");
            }

            return File(documento.DocumentoBytes, "application/octet-stream", documento.NomeArquivo);
        }
    }
}