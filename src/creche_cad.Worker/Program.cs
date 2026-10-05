using creche_cad.Data.Context;
using Microsoft.EntityFrameworkCore;
using creche_cad.Worker;

var builder = Host.CreateApplicationBuilder(args);
var dataDirectory = builder.Configuration["DataDirectory"] ?? "/data";
Directory.CreateDirectory(dataDirectory);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Data Source={Path.Combine(dataDirectory, "crechecad.db")};Default Timeout=30";
builder.Services.AddDbContext<CrecheDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "redis:6379,abortConnect=false,connectTimeout=2000";
    options.InstanceName = "crechecad:";
});
builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddHostedService<DashboardCacheInvalidationConsumer>();

await builder.Build().RunAsync();
