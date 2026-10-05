using creche_cad.Data.Context;
using creche_cad.Service.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace creche_cad.Data;

public sealed class DashboardSummaryReader(CrecheDbContext db) : IDashboardSummaryReader
{
    public async Task<DashboardSummary> ReadAsync(bool demo, int classListLimit, CancellationToken cancellationToken)
    {
        var students = await db.Alunos.CountAsync(cancellationToken);
        var teachers = await db.Professores.CountAsync(cancellationToken);
        var classes = await db.Turmas.CountAsync(cancellationToken);
        var summaries = await db.Turmas.AsNoTracking().OrderBy(classroom => classroom.Nome).Take(classListLimit)
            .Select(classroom => new DashboardClassSummary(
                classroom.Id,
                classroom.Nome,
                classroom.Metragem,
                db.Alunos.Count(student => student.TurmaId == classroom.Id)))
            .ToListAsync(cancellationToken);
        return new DashboardSummary(students, teachers, classes, demo, summaries);
    }
}
