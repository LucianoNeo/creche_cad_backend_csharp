using creche_cad.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CrecheDbContext _context;

        public DatabaseController(IConfiguration configuration, CrecheDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("check-database")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var anyRecord = _context.Turmas.ToArray();

                if (anyRecord is not null)
                {
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    string dbPath = connectionString.Substring(connectionString.IndexOf('=') + 1);
                    return Ok(new { dbPath, message = "Conexão com o banco de dados estabelecida com sucesso." });
                }
                else
                {
                    return StatusCode(500, "Nenhum registro encontrado no banco de dados.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao verificar o banco de dados: {ex.Message}");
            }
        }

        [HttpGet("backup")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                // Caminho fixo para salvar o backup
                string backupDirectory = @"C:\CrecheCadBackup";

                _context.Database.CloseConnection();

                // Criar o diretório se não existir
                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                // Obtém o caminho do banco de dados
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                string dbPath = connectionString.Substring(connectionString.IndexOf('=') + 1);

                // Verifica se o arquivo do banco de dados existe
                if (!System.IO.File.Exists(dbPath))
                    return NotFound("O arquivo do banco de dados não foi encontrado.");

                // Define o nome do arquivo de backup com a data atual
                string backupFileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                string backupPath = Path.Combine(backupDirectory, backupFileName);

                // Realiza o backup do banco de dados
                await CopyFileAsync(dbPath, backupPath);

                return Ok("Backup do banco de dados realizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar o backup do banco de dados: {ex.Message}");
            }
        }

        private async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            // Abrir o arquivo de origem para leitura
            using (var sourceStream = System.IO.File.OpenRead(sourcePath))
            {
                // Criar ou sobrescrever o arquivo de destino e abrir para gravação
                using (var destinationStream = System.IO.File.Create(destinationPath))
                {
                    // Copiar o conteúdo do arquivo de origem para o arquivo de destino
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }
    }
}