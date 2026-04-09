using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace SportsStore.OrderApi.Services;

public class RabbitMQPublisher : IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;

    public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(string queueName, T message)
    {
        await _channel!.QueueDeclareAsync(queue: queueName, durable: true,
            exclusive: false, autoDelete: false);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: queueName, body: body);

        _logger.LogInformation("Published to {Queue}: {Message}", queueName, json);
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}
