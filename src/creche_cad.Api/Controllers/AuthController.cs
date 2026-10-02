using creche_cad.Data.Context;
using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
namespace creche_cad.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(CrecheDbContext db, PasswordHasher<SchoolUser> hasher, IAntiforgery antiforgery) : ControllerBase {
 [HttpGet("csrf"), AllowAnonymous]
 public IActionResult Csrf() => Ok(new { token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken });
 [HttpPost("login"), AllowAnonymous, EnableRateLimiting("login")]
 public async Task<IActionResult> Login(LoginInput input) {
  var user = await db.Users.SingleOrDefaultAsync(u => u.Username == input.Username.Trim().ToLowerInvariant() && u.Active);
  if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, input.Password) == PasswordVerificationResult.Failed)
   return Unauthorized(new { message = "Usuário ou senha inválidos." });
  var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()), new Claim(ClaimTypes.Name,user.Username),
   new Claim(ClaimTypes.Role,user.Role), new Claim("stamp",user.SecurityStamp)], CookieAuthenticationDefaults.AuthenticationScheme);
  await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));
  return Ok(new { username=user.Username, role=user.Role });
 }
 [HttpGet("me")] public IActionResult Me() => Ok(new { username=User.Identity!.Name, role=User.FindFirstValue(ClaimTypes.Role) });
 [HttpPost("logout")] public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return NoContent(); }
 [HttpPost("change-password")]
 public async Task<IActionResult> ChangePassword(ChangePasswordInput input) {
  var user=await db.Users.SingleAsync(u => u.Id==Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
  if(hasher.VerifyHashedPassword(user,user.PasswordHash,input.CurrentPassword)==PasswordVerificationResult.Failed) return BadRequest(new { message="A senha atual está incorreta." });
  user.PasswordHash=hasher.HashPassword(user,input.NewPassword); user.SecurityStamp=Guid.NewGuid().ToString("N");
  await db.SaveChangesAsync(); await HttpContext.SignOutAsync(); return NoContent();
 }
 [HttpPost("recovery"), AllowAnonymous, EnableRateLimiting("login")]
 public async Task<IActionResult> Recovery(RecoveryInput input) {
  var name=input.Username.Trim().ToLowerInvariant();
  if(await db.Users.AnyAsync(u => u.Username==name && u.Active) && !await db.Recoveries.AnyAsync(r => r.Username==name && !r.Resolved)) {
   db.Recoveries.Add(new AccessRecovery { Username=name }); await db.SaveChangesAsync();
  }
  return Accepted(value:new { message="Solicitação registrada. A secretaria administra a recuperação de acesso." });
 }
}
public class LoginInput {
 [Required,StringLength(100)] public string Username { get; set; }="";
 [Required,StringLength(200)] public string Password { get; set; }="";
}
public class RecoveryInput { [Required,StringLength(100)] public string Username { get; set; }=""; }
public class ChangePasswordInput {
 [Required] public string CurrentPassword { get; set; }="";
 [Required,StringLength(128,MinimumLength=12)] public string NewPassword { get; set; }="";
}
