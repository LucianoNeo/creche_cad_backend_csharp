using RabbitMQ.Client;

namespace creche_cad.Worker;

internal static class RabbitSettings
{
    public const string Queue = "crechecad.school-data-changed.v1";

    public static ConnectionFactory Create(IConfiguration configuration) => new()
    {
        HostName = configuration["RabbitMq:Host"] ?? "rabbitmq",
        UserName = configuration["RabbitMq:Username"] ?? "crechecad",
        Password = configuration["RabbitMq:Password"] ?? "crechecad-local-only",
        ClientProvidedName = "crechecad:dashboard-cache-invalidator"
    };
}
