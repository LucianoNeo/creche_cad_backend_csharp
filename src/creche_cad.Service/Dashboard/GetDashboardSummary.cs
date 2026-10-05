using System.Text.Json;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace creche_cad.Service.Dashboard;

public sealed record DashboardClassSummary(Guid Id, string Nome, string? Metragem, int Count);
public sealed record DashboardSummary(int Students, int Teachers, int Classes, bool Demo, IReadOnlyList<DashboardClassSummary> Turmas);
public sealed record GetDashboardSummaryQuery(bool Demo, int ClassListLimit = 8) : IRequest<DashboardSummary>;

public interface IDashboardSummaryReader
{
    Task<DashboardSummary> ReadAsync(bool demo, int classListLimit, CancellationToken cancellationToken);
}

public sealed class GetDashboardSummaryHandler(IDashboardSummaryReader reader, IDistributedCache cache)
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummary>
{
    public async Task<DashboardSummary> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var key = $"crechecad:dashboard:v1:{request.Demo}:{request.ClassListLimit}";
        try
        {
            var cached = await cache.GetStringAsync(key, cancellationToken);
            if (cached is not null) return JsonSerializer.Deserialize<DashboardSummary>(cached)!;
        }
        catch (Exception exception) when (exception is not OperationCanceledException) { }

        var result = await reader.ReadAsync(request.Demo, request.ClassListLimit, cancellationToken);
        try
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            }, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException) { }

        return result;
    }
}

public sealed class GetDashboardSummaryValidator : AbstractValidator<GetDashboardSummaryQuery>
{
    public GetDashboardSummaryValidator() => RuleFor(query => query.ClassListLimit).InclusiveBetween(1, 8);
}
