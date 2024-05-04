using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using creche_cad.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController : ControllerBase
    {
        private readonly CrecheDbContext _context;

        public ProfessorController(CrecheDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarProfessor([FromBody] ProfessorInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var professor = new Professor
            {
                Nome = input.Nome,
                RG = input.RG,
                CPF = input.CPF,
                Endereco = input.Endereco,
                TelefonePrincipal = input.TelefonePrincipal,
                TelefoneSecundario = input.TelefoneSecundario,
                TelefoneCelular = input.TelefoneSecundario,
                Titulo = input.Titulo,
                CarteiraTrabalho = input.CarteiraTrabalho,
                DataAdmissao = input.DataAdmissao,
                DataCriacao = DateTime.Now
            };

            _context.Professores.Add(professor);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterProfessor), new { id = professor.Id }, professor);
        }

        [HttpGet]
        public IActionResult ObterProfessores()
        {
            var professores = _context.Professores.ToList().Select(p => new ProfessorDto
            {
                Id = p.Id,
                Nome = p.Nome,
                RG = p.RG,
                CPF = p.CPF,
                Endereco = p.Endereco,
                TelefonePrincipal = p.TelefonePrincipal,
                TelefoneCelular = p.TelefoneCelular,
                TelefoneSecundario = p.TelefoneSecundario,
                Titulo = p.Titulo,
                CarteiraTrabalho = p.CarteiraTrabalho,
                DataAdmissao = p.DataAdmissao,
                DataDemissao = p.DataDemissao
            });

            return Ok(professores);
        }

        [HttpGet("{id}")]
        public IActionResult ObterProfessor(Guid id)
        {
            var professor = _context.Professores.Find(id);

            if (professor == null)
            {
                return NotFound("Professor não encontrado");
            }

            var professorDto = new ProfessorDto
            {
                Id = professor.Id,
                Nome = professor.Nome,
                RG = professor.RG,
                CPF = professor.CPF,
                Endereco = professor.Endereco,
                TelefonePrincipal = professor.TelefonePrincipal,
                TelefoneCelular = professor.TelefoneCelular,
                TelefoneSecundario = professor.TelefoneSecundario,
                Titulo = professor.Titulo,
                CarteiraTrabalho = professor.CarteiraTrabalho,
                DataAdmissao = professor.DataAdmissao,
                DataDemissao = professor.DataDemissao
            };

            return Ok(professorDto);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarProfessor(Guid id, [FromBody] ProfessorInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var professorExistente = _context.Professores.Find(id);
            if (professorExistente == null)
                return NotFound("Professor não encontrado");

            professorExistente.Nome = input.Nome;
            professorExistente.RG = input.RG;
            professorExistente.CPF = input.CPF;
            professorExistente.Endereco = input.Endereco;
            professorExistente.TelefonePrincipal = input.TelefonePrincipal;
            professorExistente.TelefoneSecundario = input.TelefoneSecundario;
            professorExistente.TelefoneCelular = input.TelefoneSecundario;
            professorExistente.Titulo = input.Titulo;
            professorExistente.CarteiraTrabalho = input.CarteiraTrabalho;
            professorExistente.DataAdmissao = input.DataAdmissao;
            professorExistente.DataDemissao = input.DataDemissao;

            _context.SaveChanges();

            return Ok(new { message = "Professor atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarProfessor(Guid id)
        {
            var professor = _context.Professores.Find(id);
            if (professor == null)
                return NotFound("Professor não encontrado");

            _context.Professores.Remove(professor);
            _context.SaveChanges();

            return Ok(new { message = "Professor deletado com sucesso" });
        }
    }
}