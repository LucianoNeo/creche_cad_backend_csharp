using creche_cad.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace creche_cad.Controllers;
[ApiController, Route("api/database"), Authorize(Roles = "Administrator")]
public class DatabaseController(CrecheDbContext db, IConfiguration config) : ControllerBase {
    [HttpGet("check-database")]
    public async Task<IActionResult> CheckDatabase(CancellationToken ct) =>
        await db.Database.CanConnectAsync(ct) ? Ok(new { status = "ok", demo = config.GetValue<bool>("Demo:Enabled") }) : StatusCode(503);
    [HttpGet("backup")]
    public async Task<IActionResult> Backup(CancellationToken ct) {
        var temporary = Path.Combine(Path.GetTempPath(), $"crechecad-{Guid.NewGuid():N}.db");
        try {
            await db.Database.OpenConnectionAsync(ct);
            using (var destination = new SqliteConnection($"Data Source={temporary}")) {
                await destination.OpenAsync(ct);
                ((SqliteConnection)db.Database.GetDbConnection()).BackupDatabase(destination);
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(temporary, ct);
            return File(bytes, "application/octet-stream", $"crechecad-{DateTime.UtcNow:yyyyMMdd-HHmmss}.db");
        } finally { if (System.IO.File.Exists(temporary)) System.IO.File.Delete(temporary); }
    }
}
