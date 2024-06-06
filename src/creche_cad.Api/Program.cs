using creche_cad.Api.Helpers;
using creche_cad.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CrecheDbContext>(options =>
    options.UseSqlite(connectionString));

// Configura��o dos servi�os da aplica��o
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");

// Configura��o para servir arquivos est�ticos da pasta dist
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Obter o ambiente de execu��o
var env = app.Environment;

// Aplicar migra��es ao banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CrecheDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao aplicar migra��es: " + ex.Message);
    }
}

// Configura��o do middleware da aplica��o
app.UseCors("AllowLocalhost3000");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "client-app";
    if (env.IsDevelopment())
    {
        // Launch development server for Nuxt
        spa.UseNuxtDevelopmentServer();
    }
});

// Abrir o navegador automaticamente na porta 5210
var url = "http://localhost:5210";
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao abrir o navegador: " + ex.Message);
    }
});

app.Run("http://localhost:5210");
