using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SecondMicroserviceRabbitMQ.Services
{
    public class MessageListener : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "demo-queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received: {message}");
            };

            channel.BasicConsume(queue: "demo-queue",
                                 autoAck: true,
                                 consumer: consumer);

            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }

                channel.Close();
                connection.Close();
            }, stoppingToken);
        }
    }
}
