using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace creche_cad.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddSchoolService(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
