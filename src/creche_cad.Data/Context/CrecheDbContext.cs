using creche_cad.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Data.Context
{
    public class CrecheDbContext : DbContext
    {
        public CrecheDbContext(DbContextOptions<CrecheDbContext> options)
            : base(options)
        {
        }

        public DbSet<Turma> Turmas { get; set; }
    }
}