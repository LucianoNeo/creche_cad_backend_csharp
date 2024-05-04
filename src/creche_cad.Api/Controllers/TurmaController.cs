using creche_cad.Data.Context;
using creche_cad.Domain.Dtos;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurmaController : ControllerBase
    {
        private readonly CrecheDbContext _context;

        public TurmaController(CrecheDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarTurma([FromBody] TurmaInputModel input)
        {
            if (string.IsNullOrEmpty(input.Nome))
                return BadRequest("O nome da turma é obrigatório");

            var turma = new Turma
            {
                Nome = input.Nome,
                DataCriacao = DateTime.Now
            };

            _context.Turmas.Add(turma);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterTurma), new { id = turma.Id }, new { id = turma.Id, input.Nome });
        }

        [HttpGet]
        public IActionResult ObterTurmas()
        {
            var turmas = _context.Turmas.ToList().Select(t => new TurmaDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Metragem = t.Metragem
            }).ToList();

            return Ok(turmas);
        }

        [HttpGet("{id}")]
        public IActionResult ObterTurma(Guid id)
        {
            var turma = _context.Turmas.Find(id);

            if (turma == null)
                return NotFound("Turma não encontrada");

            var turmaOutput = new TurmaDto
            {
                Id = turma.Id,
                Metragem = turma.Metragem,
                Nome = turma.Nome
            };
            return Ok(turmaOutput);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarTurma(Guid id, [FromBody] TurmaInputModel input)
        {
            if (string.IsNullOrEmpty(input.Nome))
                return BadRequest("O nome da turma é obrigatório");

            var turma = _context.Turmas.Find(id);
            if (turma == null)
                return NotFound("Turma não encontrada");

            turma.Nome = input.Nome;
            turma.Metragem = input.Metragem;
            turma.DataAtualizacao = DateTime.Now;
            _context.SaveChanges();

            return Ok(new { message = "Turma atualizada com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarTurma(Guid id)
        {
            var turma = _context.Turmas.Find(id);
            if (turma == null)
                return NotFound("Turma não encontrada");

            _context.Turmas.Remove(turma);
            _context.SaveChanges();

            return Ok(new { message = "Turma deletada com sucesso" });
        }
    }
}