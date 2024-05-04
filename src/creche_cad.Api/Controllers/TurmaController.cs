using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurmasController : ControllerBase
    {
        private readonly CrecheDbContext _context;

        public TurmasController(CrecheDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarTurma([FromBody] TurmaInputModel input)
        {
            if (string.IsNullOrEmpty(input.Nome))
                return BadRequest("O nome da turma é obrigatório");

            var turma = new Turma { Nome = input.Nome };
            _context.Turmas.Add(turma);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterTurma), new { id = turma.Id }, new { id = turma.Id, input.Nome });
        }

        [HttpGet]
        public IActionResult ObterTurmas()
        {
            var turmas = _context.Turmas.ToList();
            return Ok(turmas);
        }

        [HttpGet("{id}")]
        public IActionResult ObterTurma(long id)
        {
            var turma = _context.Turmas.Find(id);
            if (turma == null)
                return NotFound("Turma não encontrada");

            return Ok(turma);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarTurma(long id, [FromBody] TurmaInputModel input)
        {
            if (string.IsNullOrEmpty(input.Nome))
                return BadRequest("O nome da turma é obrigatório");

            var turma = _context.Turmas.Find(id);
            if (turma == null)
                return NotFound("Turma não encontrada");

            turma.Nome = input.Nome;
            _context.SaveChanges();

            return Ok(new { message = "Turma atualizada com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarTurma(long id)
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