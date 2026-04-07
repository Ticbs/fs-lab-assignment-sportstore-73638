using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using SportsStore.Shared.Events;

namespace SportsStore.ShippingService.Workers;

public class ShippingWorker : BackgroundService
{
    private readonly ILogger<ShippingWorker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public ShippingWorker(ILogger<ShippingWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "shipping-create",
            durable: true, exclusive: false, autoDelete: false);

        _logger.LogInformation("ShippingWorker started, waiting for messages...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var payment = JsonConvert.DeserializeObject<PaymentProcessedEvent>(message);

            if (payment == null) return;

            _logger.LogInformation("Creating shipment for Order {OrderId}", payment.OrderId);

            var result = new ShippingCreatedEvent
            {
                OrderId = payment.OrderId,
                Success = true,
                TrackingReference = $"TRACK-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                EstimatedDispatch = DateTime.UtcNow.AddDays(2),
                Message = "Shipment created successfully"
            };

            _logger.LogInformation("Shipment created for Order {OrderId} — Tracking: {Tracking}",
                payment.OrderId, result.TrackingReference);

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue: "shipping-create",
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
