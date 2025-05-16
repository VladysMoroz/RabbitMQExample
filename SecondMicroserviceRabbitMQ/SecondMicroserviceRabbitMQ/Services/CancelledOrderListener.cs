using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SecondMicroserviceRabbitMQ.Services
{
    public class CancelledOrderListener : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("order-exchange", ExchangeType.Topic);
            channel.QueueDeclare("cancelled-orders-queue", durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind("cancelled-orders-queue", "order-exchange", "order.cancelled");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Cancelled] {message}");
            };

            channel.BasicConsume(queue: "cancelled-orders-queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
