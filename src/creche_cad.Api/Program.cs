using creche_cad.Data.Context;
using creche_cad.Data;
using creche_cad.Api.Security;
using creche_cad.Api.Services;
using creche_cad.Service;
using creche_cad.Service.Dashboard;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using creche_cad.Domain.Entities;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var dataDirectory = builder.Configuration["DataDirectory"] ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDirectory, "keys")));
builder.Services.AddDbContext<CrecheDbContext>(options => options
    .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? $"Data Source={Path.Combine(dataDirectory, "crechecad.db")}")
);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false,connectTimeout=2000";
    options.InstanceName = "crechecad:";
});
builder.Services.AddSchoolService();
builder.Services.AddScoped<IDashboardSummaryReader, DashboardSummaryReader>();
builder.Services.AddScoped<PasswordHasher<SchoolUser>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.Cookie.Name = "crechecad.session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Configuration.GetValue<bool>("Demo:Enabled") ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.Events.OnValidatePrincipal=async ctx=>{
        var db=ctx.HttpContext.RequestServices.GetRequiredService<CrecheDbContext>();
        var id=ctx.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var user=Guid.TryParse(id,out var uid)?await db.Users.AsNoTracking().SingleOrDefaultAsync(u=>u.Id==uid):null;
        if(user is null||!user.Active||user.SecurityStamp!=ctx.Principal?.FindFirstValue("stamp")) ctx.RejectPrincipal();
    };
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = false;
    options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
});
builder.Services.AddAuthorization();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
    options.Filters.Add(new StaffWriteFilter());
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add<FluentValidationActionFilter>();
});
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(options => {
    options.RejectionStatusCode = 429;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<CrecheDbContext>();
    await context.Database.MigrateAsync();
    if(!await context.Users.AnyAsync()) {
        var user=new SchoolUser { Username=(builder.Configuration["Admin:Username"]??throw new InvalidOperationException("Configure Admin__Username.")).ToLowerInvariant(),Role="Administrator" };
        var password=builder.Configuration["Admin:Password"]??throw new InvalidOperationException("Configure Admin__Password.");
        if(password.Length<12)throw new InvalidOperationException("Use a password of at least 12 characters.");
        user.PasswordHash=scope.ServiceProvider.GetRequiredService<PasswordHasher<SchoolUser>>().HashPassword(user,password);
        context.Users.Add(user); await context.SaveChangesAsync();
    }
    if(args.Contains("--reset-admin")) {
        var name=(builder.Configuration["Admin:Username"]??"").ToLowerInvariant();var user=await context.Users.SingleOrDefaultAsync(u=>u.Username==name)??throw new InvalidOperationException("Administrator not found.");
        var password=builder.Configuration["Admin:Password"]??"";if(password.Length<12)throw new InvalidOperationException("Password requires 12 characters.");
        user.PasswordHash=scope.ServiceProvider.GetRequiredService<PasswordHasher<SchoolUser>>().HashPassword(user,password);user.SecurityStamp=Guid.NewGuid().ToString("N");user.Active=true;user.Role="Administrator";await context.SaveChangesAsync();Console.WriteLine("Administrator access restored.");return;
    }
    if (builder.Configuration.GetValue<bool>("Demo:Enabled")) await DemoData.SeedAsync(context);
}
app.UseExceptionHandler();
app.Use(async (ctx, next) => {
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["Cache-Control"] = "no-store";
    await next();
});
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (ctx,next)=>{
    ctx.RequestServices.GetRequiredService<CrecheDbContext>().AuditActor=ctx.User.Identity?.Name??"anonymous";
    await next();
});
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapControllers();
app.Run();
public partial class Program { }
