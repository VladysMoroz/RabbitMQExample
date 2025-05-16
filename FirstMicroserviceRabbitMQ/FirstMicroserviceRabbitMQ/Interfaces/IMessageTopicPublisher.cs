namespace FirstMicroserviceRabbitMQ.Interfaces
{
    public interface IMessageTopicPublisher
    {
        void Publish(object message, string routingKey);
    }
}
