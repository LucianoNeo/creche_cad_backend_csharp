using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace creche_cad.Data.Context;
public class DesignTimeFactory : IDesignTimeDbContextFactory<CrecheDbContext> {
 public CrecheDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<CrecheDbContext>().UseSqlite("Data Source=design.db").Options);
}
