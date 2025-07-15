namespace AuthService.Infra.Messaging.RabbitMq;

public interface IEventPublisher
{
   Task PublishUserRegistered(Guid userId, string email);
}