using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace creche_cad.Worker;

public sealed class DashboardCacheInvalidationConsumer(
    IConfiguration configuration,
    IDistributedCache cache,
    ILogger<DashboardCacheInvalidationConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = RabbitSettings.Create(configuration);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                await channel.QueueDeclareAsync(RabbitSettings.Queue, durable: true, exclusive: false,
                    autoDelete: false, arguments: null, cancellationToken: stoppingToken);
                await channel.BasicQosAsync(0, 10, false, stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, delivery) =>
                {
                    try
                    {
                        var message = JsonDocument.Parse(delivery.Body);
                        if (message.RootElement.TryGetProperty("eventType", out var type) &&
                            type.GetString() == "school.summary.invalidate.v1")
                        {
                            foreach (var isDemo in new[] { false, true })
                                await cache.RemoveAsync($"crechecad:dashboard:v1:{isDemo}:8", stoppingToken);
                        }
                        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                    catch (Exception exception)
                    {
                        logger.LogWarning(exception, "Dashboard cache invalidation failed; RabbitMQ will redeliver it.");
                        await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: true, stoppingToken);
                    }
                };

                await channel.BasicConsumeAsync(RabbitSettings.Queue, autoAck: false, consumer, stoppingToken);
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "RabbitMQ is unavailable; the cache consumer is reconnecting.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
