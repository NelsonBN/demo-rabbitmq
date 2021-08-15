using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.Topic.Produce
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
                    exchange: "Notifications",
                    type: "topic"
                );

                // Create queues
                channel.QueueDeclare(
                    queue: "SMS",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                channel.QueueDeclare(
                    queue: "Push",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Attach queue to exchange
                channel.QueueBind(
                    queue: "SMS",
                    exchange: "Notifications",
                    routingKey: "Products.#"
                );
                channel.QueueBind(
                    queue: "Push",
                    exchange: "Notifications",
                    routingKey: "*.Promotions.*"
                );
                channel.QueueBind(
                    queue: "Push",
                    exchange: "Notifications",
                    routingKey: "Stock.#"
                );

                var messageId = 0;
                while(true)
                {
                    messageId++;

                    // *************************

                    var message = $"Produce > {produceId} - {messageId} > PC Before 513.19, Now 499.98";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "Notifications", routingKey: "Products.Promotions.Weekend.Flash", basicProperties: null, body: body);
                    Console.WriteLine(">>> {0}", message);

                    // *************************

                    message = $"Produce {produceId} - {messageId} > Mobile Before 213.19, Now 179.48";
                    body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "Notifications", routingKey: "Stock.Promotions.Blackfriday", basicProperties: null, body: body);
                    Console.WriteLine(">>> {0}", message);

                    // *************************

                    message = $"Produce {produceId} - {messageId} > Headphones available again";
                    body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "Notifications", routingKey: "Stock.Products.New", basicProperties: null, body: body);
                    Console.WriteLine(">>> {0}", message);

                    // *************************

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