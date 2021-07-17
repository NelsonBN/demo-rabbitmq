using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.ConsumerAPP
{
    public class Program
    {
        static void Main(string[] _)
        {
            var consumerId = Guid.NewGuid();

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "myqueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (_, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"--> Consume {consumerId} Received {message}");
                };

                channel.BasicConsume(
                    queue: "myqueue",
                    autoAck: true,
                    consumer: consumer
                );

                Console.WriteLine($"ConsumerAPP {consumerId} runing...");
                Console.ReadLine();
            }
        }
    }
}