using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.ProduceAPP
{
    public class Program
    {
        private const int TIME_PAUSE = 10;
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
                channel.QueueDeclare(
                    queue: "myqueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var messageId = 0;
                while(true)
                {
                    messageId++;
                    var message = $"Produce {produceId} - {messageId} > I am Nelson Nobre";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: "myqueue",
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine("--> Sent {0}", message);

                    if(TIME_PAUSE > 0)
                    {
                        Console.WriteLine($"\tWaiting {TIME_PAUSE}ms...");
                        Thread.Sleep(TIME_PAUSE);
                    }
                }
            }

            Console.WriteLine("ProduceAPP runing...");
        }
    }
}