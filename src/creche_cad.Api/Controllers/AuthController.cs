using creche_cad.Api.Security;
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
public class AuthController(AdminCredentials credentials, IAntiforgery antiforgery) : ControllerBase {
    [HttpGet("csrf"), AllowAnonymous]
    public IActionResult Csrf() => Ok(new { token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken });
    [HttpPost("login"), AllowAnonymous, EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginInput input) {
        if (!credentials.Verify(input.Username, input.Password)) return Unauthorized(new { message = "Usuário ou senha inválidos." });
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, credentials.Username), new Claim(ClaimTypes.Role, "Administrator")], CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return Ok(new { username = credentials.Username });
    }
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { username = User.Identity!.Name });
    [HttpPost("logout")]
    public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return NoContent(); }
}
public sealed class LoginInput {
    [Required, StringLength(100)] public string Username { get; set; } = "";
    [Required, StringLength(200)] public string Password { get; set; } = "";
}
