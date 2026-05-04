using Confluent.Kafka;
using Dapper;
using Microsoft.Extensions.Logging;
using ProductReadService.Domain;
using ProductReadService.Infrastructure.Data;
using System.Text.Json;

namespace ProductReadService.Infrastructure.Kafka;

public class ProductConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly DbConnectionFactory _factory;
    private readonly ILogger<ProductConsumer> _logger;

    public ProductConsumer(IConfiguration config, DbConnectionFactory factory, ILogger<ProductConsumer> logger)
    {
        _config = config;
        _factory = factory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return Task.Run(() => Run(ct), ct);
    }

    private void Run(CancellationToken ct)
    {
        var topic = _config["Kafka:Topic"] ?? "product-events";
        var config = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupId"] ?? "product-read-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogWarning("Kafka consumer error: {Error}", e))
            .Build();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                consumer.Subscribe(topic);
                break;
            }
            catch (KafkaException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                _logger.LogWarning("Kafka topic '{Topic}' not available yet. Retrying in 5 seconds...", topic);
                try
                {
                    Task.Delay(TimeSpan.FromSeconds(5), ct).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var msg = consumer.Consume(ct);

                    if (msg?.Message?.Value == null)
                        continue;

                    ProductCreatedEvent? evt;
                    try
                    {
                        evt = JsonSerializer.Deserialize<ProductCreatedEvent>(msg.Message.Value);
                    }
                    catch (JsonException)
                    {
                        _logger.LogWarning("Skipping invalid Kafka message.");
                        continue;
                    }

                    if (evt == null)
                        continue;

                    using var conn = _factory.Create();
                    conn.Execute(@"
                        INSERT INTO ProductView (Id, Name, Price)
                        VALUES (@Id, @Name, @Price)
                    ", evt);

                    consumer.Commit(msg);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning(ex, "Kafka consume error for topic '{Topic}'. Retrying in 5 seconds...", topic);
                    try
                    {
                        Task.Delay(TimeSpan.FromSeconds(5), ct).GetAwaiter().GetResult();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // graceful shutdown
        }
        finally
        {
            consumer.Close();
        }
    }
}