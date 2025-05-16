using FirstMicroserviceRabbitMQ.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace FirstMicroserviceRabbitMQ.Services
{
    public class TopicMessagePublisher : IMessageTopicPublisher
    {
        public void Publish(object message, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("order-exchange", ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(
                exchange: "order-exchange",
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}
