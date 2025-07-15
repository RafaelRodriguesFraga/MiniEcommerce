using System.Text;
using System.Text.Json;
using AuthService.Infra.Messaging.Config;
using AuthService.Infra.Messaging.RabbitMq;
using RabbitMQ.Client;

namespace AuthService.Infra.Messaging;

public class EventPublisher : IEventPublisher, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string ExchangeName = "auth.exchange";

    public async Task InitAsync(string? host)
    {
        _connection = await RabbitMqConnectionFactory.CreateConnectionAsync(host);
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true);
    }

    public async Task PublishUserRegistered(Guid userId, string email)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel not initialized");
        }

        var message = JsonSerializer.Serialize(new
        {
            UserId = userId,
            Email = email,
            Event = "UserRegistered",
            Timestamp = DateTime.UtcNow
        });
        
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: ExchangeName, routingKey: "", body: body);
    }


    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}