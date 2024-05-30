using creche_cad.Data.Context;
using creche_cad.Domain.Dtos;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly CrecheDbContext _context;

        public AlunoController(CrecheDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarAluno([FromBody] AlunoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var aluno = new Aluno
            {
                Nome = input.Nome,
                TurmaId = input.TurmaId,
                DataNascimento = input.DataNascimento,
                NomePai = input.NomePai,
                NomeMae = input.NomeMae,
                Endereco = input.Endereco,
                Telefone = input.Telefone,
                DataCriacao = DateTime.Now,
                Turma = _context.Turmas.Where(t => t.Id == input.TurmaId).First(),
            };

            _context.Alunos.Add(aluno);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterAluno), new { id = aluno.Id }, aluno);
        }

        [HttpGet]
        public IActionResult ObterAlunos()
        {
            var alunosComTurmaNome = _context.Alunos
                .Select(a => new
                {
                    Aluno = a,
                })
                .ToList()
                .Select(a => new AlunoDto
                {
                    Id = a.Aluno.Id,
                    Nome = a.Aluno.Nome,
                    DataNascimento = a.Aluno.DataNascimento,
                    NomePai = a.Aluno.NomePai,
                    NomeMae = a.Aluno.NomeMae,
                    Endereco = a.Aluno.Endereco,
                    Telefone = a.Aluno.Telefone,
                    TurmaId = a.Aluno.TurmaId,
                    TurmaNome = _context.Turmas.Where(t => t.Id == a.Aluno.TurmaId).First().Nome,
                });

            return Ok(alunosComTurmaNome);
        }


        [HttpGet("{id}")]
        public IActionResult ObterAluno(Guid id)
        {
            var aluno = _context.Alunos.Find(id);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado");
            }
            
            // Busca o nome da turma associada ao aluno
            var turma = _context.Turmas.FirstOrDefault(t => t.Id == aluno.TurmaId);
            var turmaNome = turma != null ? turma.Nome : string.Empty;

            var alunoOutput = new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                NomePai = aluno.NomePai,
                NomeMae = aluno.NomeMae,
                Endereco = aluno.Endereco,
                Telefone = aluno.Telefone,
                TurmaId = aluno.TurmaId,
                TurmaNome = turmaNome
            };

            return Ok(alunoOutput);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarAluno(Guid id, [FromBody] AlunoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var alunoExistente = _context.Alunos.Find(id);
            if (alunoExistente == null)
                return NotFound("Aluno não encontrado");

            alunoExistente.Nome = input.Nome;
            alunoExistente.DataNascimento = input.DataNascimento;
            alunoExistente.NomePai = input.NomePai;
            alunoExistente.NomeMae = input.NomeMae;
            alunoExistente.Endereco = input.Endereco;
            alunoExistente.Telefone = input.Telefone;
            alunoExistente.TurmaId = input.TurmaId;

            _context.SaveChanges();

            return Ok(new { message = "Aluno atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarAluno(Guid id)
        {
            var aluno = _context.Alunos.Find(id);
            if (aluno == null)
                return NotFound("Aluno não encontrado");

            _context.Alunos.Remove(aluno);
            _context.SaveChanges();

            return Ok(new { message = "Aluno deletado com sucesso" });
        }
    }
}