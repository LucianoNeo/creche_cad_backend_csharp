using System.Text;
using System.Text.Json;
using creche_cad.Data.Context;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace creche_cad.Worker;

public sealed class OutboxPublisher(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<OutboxPublisher> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = RabbitSettings.Create(configuration);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
                    stoppingToken);
                await channel.QueueDeclareAsync(RabbitSettings.Queue, durable: true, exclusive: false,
                    autoDelete: false, arguments: null, cancellationToken: stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<CrecheDbContext>();
                    var messages = await db.Outbox.Where(message => message.PublishedAtUtc == null)
                        .OrderBy(message => message.OccurredAtUtc).Take(25).ToListAsync(stoppingToken);
                    foreach (var message in messages)
                    {
                        // The event carries no student, family or document data.
                        var body = JsonSerializer.SerializeToUtf8Bytes(new { eventType = message.EventType });
                        var properties = new BasicProperties
                        {
                            ContentType = "application/json",
                            DeliveryMode = DeliveryModes.Persistent,
                            MessageId = message.Id.ToString("N")
                        };
                        await channel.BasicPublishAsync("", RabbitSettings.Queue, mandatory: true,
                            basicProperties: properties, body, cancellationToken: stoppingToken);
                        message.PublishedAtUtc = DateTime.UtcNow;
                    }
                    if (messages.Count > 0) await db.SaveChangesAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(messages.Count == 0 ? 3 : 1), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "The school change outbox could not be sent; pending messages will be retried.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
