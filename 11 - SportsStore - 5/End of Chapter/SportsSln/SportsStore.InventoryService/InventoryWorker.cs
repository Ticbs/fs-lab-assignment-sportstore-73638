using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using SportsStore.Shared.Events;

namespace SportsStore.InventoryService.Workers;

public class InventoryWorker : BackgroundService
{
    private readonly ILogger<InventoryWorker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public InventoryWorker(ILogger<InventoryWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "inventory-check",
            durable: true, exclusive: false, autoDelete: false);

        await _channel.QueueDeclareAsync(queue: "payment-process",
            durable: true, exclusive: false, autoDelete: false);

        _logger.LogInformation("InventoryWorker started, waiting for messages...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonConvert.DeserializeObject<OrderSubmittedEvent>(message);

            if (order == null) return;

            _logger.LogInformation("Received inventory check for Order {OrderId}", order.OrderId);

            // Simula verificacao de stock — 90% de sucesso
            var success = new Random().Next(1, 10) > 1;

            var result = new InventoryCheckedEvent
            {
                OrderId = order.OrderId,
                Success = success,
                Message = success ? "Stock confirmed" : "Insufficient stock"
            };

            var resultJson = JsonConvert.SerializeObject(result);
            var resultBody = Encoding.UTF8.GetBytes(resultJson);

            if (success)
            {
                // Publica para o PaymentService
                await _channel.BasicPublishAsync(exchange: string.Empty,
                    routingKey: "payment-process", body: resultBody);
                _logger.LogInformation("Inventory confirmed for Order {OrderId}, sent to payment", order.OrderId);
            }
            else
            {
                _logger.LogWarning("Inventory FAILED for Order {OrderId}", order.OrderId);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue: "inventory-check",
            autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
