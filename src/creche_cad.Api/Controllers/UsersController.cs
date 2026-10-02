using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
namespace creche_cad.Controllers;
[ApiController,Route("api/users"),Authorize(Roles="Administrator")]
public class UsersController(CrecheDbContext db,PasswordHasher<SchoolUser> hasher):ControllerBase {
 [HttpGet] public async Task<IActionResult> List(int page=1,int pageSize=10,string? q=null) {
 page=Math.Clamp(page,1,100000);pageSize=Math.Clamp(pageSize,1,100);var query=db.Users.AsNoTracking();if(!string.IsNullOrWhiteSpace(q))query=query.Where(u=>u.Username.Contains(q));return Ok(new{items=await query.OrderBy(u=>u.Username).Skip((page-1)*pageSize).Take(pageSize).Select(u=>new{u.Id,u.Username,u.Role,u.Active}).ToListAsync(),total=await query.CountAsync()});
 }
 [HttpPost] public async Task<IActionResult> Create(UserInput input) {
  var name=input.Username.Trim().ToLowerInvariant();
  if(!new[]{"Administrator","Secretary","Viewer"}.Contains(input.Role)) return BadRequest();
  if(await db.Users.AnyAsync(u=>u.Username==name)) return Conflict(new {message="Esse usuÃ¡rio jÃ¡ existe."});
  var user=new SchoolUser {Username=name,Role=input.Role}; user.PasswordHash=hasher.HashPassword(user,input.Password);
  db.Users.Add(user); await db.SaveChangesAsync(); return Created($"/api/users/{user.Id}",new{user.Id,user.Username,user.Role});
 }
 [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id,UserRoleInput input) {
  if(!new[]{"Administrator","Secretary","Viewer"}.Contains(input.Role)) return BadRequest();
  var user=await db.Users.FindAsync(id); if(user is null)return NotFound();
  if(user.Id==Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) && (!input.Active||input.Role!="Administrator")) return BadRequest(new{message="Mantenha sua conta administrativa ativa."});
  user.Role=input.Role; user.Active=input.Active; user.SecurityStamp=Guid.NewGuid().ToString("N"); await db.SaveChangesAsync(); return NoContent();
 }
 [HttpPost("{id:guid}/reset-password")] public async Task<IActionResult> Reset(Guid id,ResetInput input) {
  var user=await db.Users.FindAsync(id); if(user is null)return NotFound();
  user.PasswordHash=hasher.HashPassword(user,input.Password); user.SecurityStamp=Guid.NewGuid().ToString("N");
  foreach(var request in await db.Recoveries.Where(r=>r.Username==user.Username&&!r.Resolved).ToListAsync()) request.Resolved=true;
  await db.SaveChangesAsync(); return NoContent();
 }
 [HttpGet("recoveries")] public async Task<IActionResult> Recoveries() => Ok(await db.Recoveries.AsNoTracking().Where(r=>!r.Resolved).OrderBy(r=>r.RequestedAt).ToListAsync());
 [HttpGet("audit")] public async Task<IActionResult> Audit(int page=1) => Ok(new {items=await db.Audit.AsNoTracking().OrderByDescending(a=>a.Id).Skip((Math.Max(1,page)-1)*25).Take(25).ToListAsync(),total=await db.Audit.CountAsync()});
}
public class UserInput {
 [Required,StringLength(100)] public string Username{get;set;}="";
 [Required,StringLength(128,MinimumLength=12)] public string Password{get;set;}="";
 [Required] public string Role{get;set;}="Secretary";
}
public record UserRoleInput(string Role,bool Active);
public class ResetInput { [Required,StringLength(128,MinimumLength=12)] public string Password{get;set;}=""; }
