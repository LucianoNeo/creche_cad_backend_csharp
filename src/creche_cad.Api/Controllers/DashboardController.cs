using creche_cad.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace creche_cad.Controllers;
[ApiController,Route("api/dashboard")]
public class DashboardController(CrecheDbContext db,IConfiguration config):ControllerBase {
 [HttpGet] public async Task<IActionResult> Get() => Ok(new {students=await db.Alunos.CountAsync(),teachers=await db.Professores.CountAsync(),classes=await db.Turmas.CountAsync(),demo=config.GetValue<bool>("Demo:Enabled"),
  turmas=await db.Turmas.AsNoTracking().OrderBy(t=>t.Nome).Take(8).Select(t=>new{t.Id,t.Nome,t.Metragem,count=db.Alunos.Count(a=>a.TurmaId==t.Id)}).ToListAsync()});
}
