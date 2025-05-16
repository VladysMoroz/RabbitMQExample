using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SecondMicroserviceRabbitMQ.Services
{
    public class UpdatedOrderListener : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("order-exchange", ExchangeType.Topic);
            channel.QueueDeclare("updated-orders-queue", durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind("updated-orders-queue", "order-exchange", "order.updated");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Updated] {message}");
            };

            channel.BasicConsume(queue: "updated-orders-queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
