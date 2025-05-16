using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SecondMicroserviceRabbitMQ.Services
{
    public class CreatedOrderListener : BackgroundService // З РЕАЛІЗОВАНИМ Dead Letter Queue
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // 🌀 Основний Exchange для публікацій
            channel.ExchangeDeclare("order-exchange", ExchangeType.Topic);

            // 💀 Dead Letter Exchange — буде отримувати невдалі повідомлення
            channel.ExchangeDeclare("order-dlx", ExchangeType.Direct);

            // 🛠️ Аргументи для основної черги: куди відправляти dead-lettered повідомлення
            var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "order-dlx" },
            { "x-dead-letter-routing-key", "order.created.dlx" }
        };

            // 📦 Основна черга з прив'язкою до DLX
            channel.QueueDeclare(
                queue: "created-orders-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs
            );

            // 🔗 Прив'язуємо основну чергу до основного exchange
            channel.QueueBind("created-orders-queue", "order-exchange", "order.created");

            // 📦 DLQ — черга для обробки невдалих повідомлень
            channel.QueueDeclare("created-orders-dlq", durable: true, exclusive: false, autoDelete: false);

            // 🔗 Прив'язка DLQ до DLX
            channel.QueueBind("created-orders-dlq", "order-dlx", "order.created.dlx");

            // 🧾 Споживач повідомлень з основної черги
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // 🧾 Просто виводимо отримане повідомлення (без помилок чи nack)
                Console.WriteLine($"[Created] {message}");

                // 🔁 Ми не викликаємо BasicNack, тому DLQ наразі не використовується.
            };

            // ✅ Автоматичне підтвердження повідомлень (без переадресації в DLQ)
            channel.BasicConsume(queue: "created-orders-queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
