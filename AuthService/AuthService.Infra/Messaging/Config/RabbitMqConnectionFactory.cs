using RabbitMQ.Client;

namespace AuthService.Infra.Messaging.Config;

public static class RabbitMqConnectionFactory
{
    public static async Task<IConnection> CreateConnectionAsync(string? rabbitMqHost)
    {
        var factory = new ConnectionFactory() { HostName = rabbitMqHost };
        
        return await factory.CreateConnectionAsync();
    }
}