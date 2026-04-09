using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using SportsStore.Shared.Events;

namespace SportsStore.PaymentService.Workers;

public class PaymentWorker : BackgroundService
{
    private readonly ILogger<PaymentWorker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public PaymentWorker(ILogger<PaymentWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "payment-process",
            durable: true, exclusive: false, autoDelete: false);

        await _channel.QueueDeclareAsync(queue: "shipping-create",
            durable: true, exclusive: false, autoDelete: false);

        _logger.LogInformation("PaymentWorker started, waiting for messages...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var inventory = JsonConvert.DeserializeObject<InventoryCheckedEvent>(message);

            if (inventory == null) return;

            _logger.LogInformation("Processing payment for Order {OrderId}", inventory.OrderId);

            // Simula pagamento — 85% de sucesso
            var success = new Random().Next(1, 100) > 15;

            var result = new PaymentProcessedEvent
            {
                OrderId = inventory.OrderId,
                Success = success,
                TransactionId = success ? Guid.NewGuid().ToString() : string.Empty,
                Message = success ? "Payment approved" : "Payment rejected"
            };

            var resultJson = JsonConvert.SerializeObject(result);
            var resultBody = Encoding.UTF8.GetBytes(resultJson);

            if (success)
            {
                await _channel.BasicPublishAsync(exchange: string.Empty,
                    routingKey: "shipping-create", body: resultBody);
                _logger.LogInformation("Payment approved for Order {OrderId}, sent to shipping", inventory.OrderId);
            }
            else
            {
                _logger.LogWarning("Payment REJECTED for Order {OrderId}", inventory.OrderId);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue: "payment-process",
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
