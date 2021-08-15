using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.Direct.Produce
{
    public class Program
    {
        private const int TIME_PAUSE = 0;
        static void Main(string[] _)
        {
            var produceId = Guid.NewGuid();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                // Create exchange
                channel.ExchangeDeclare(
                    exchange: "Products",
                    type: "direct"
                );

                // Create queues
                channel.QueueDeclare(
                    queue: "Cart",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                channel.QueueDeclare(
                    queue: "Notifications",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Attach queue to exchange
                channel.QueueBind(
                    queue: "Cart",
                    exchange: "Products",
                    routingKey: "Save"
                );
                channel.QueueBind(
                    queue: "Notifications",
                    exchange: "Products",
                    routingKey: "Save"
                );
                channel.QueueBind(
                    queue: "Notifications",
                    exchange: "Products",
                    routingKey: "Notify"
                );

                var messageId = 0;
                while(true)
                {
                    messageId++;
                    var message = $"Produce > {produceId} - {messageId} > Buy a product";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "Products",
                        routingKey: "Save",
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine(">>> Save > {0}", message);

                    // *************************

                    message = $"Produce {produceId} - {messageId} > New promotion";
                    body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "Products",
                        routingKey: "Notify",
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine(">>> Notify > {0}", message);

                    if(messageId == 1)
                    {
                        break;
                    }

                    if(TIME_PAUSE > 0)
                    {
                        Console.WriteLine($"\tWaiting {TIME_PAUSE}ms...");
                        Thread.Sleep(TIME_PAUSE);
                    }
                }
            }

            Console.WriteLine("Direct produce > PRESS A KEY TO CONTINUE...");
        }
    }
}