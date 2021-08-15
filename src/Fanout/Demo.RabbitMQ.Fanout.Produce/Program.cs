using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.Fanout.Produce
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
                // Create exchange
                channel.ExchangeDeclare(
                    exchange: "Events",
                    type: "fanout"
                );

                // Create queues
                channel.QueueDeclare(
                    queue: "SaveEvents",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                channel.QueueDeclare(
                    queue: "SendNotifications",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Attach queue to exchange
                channel.QueueBind(
                    queue: "SaveEvents",
                    exchange: "Events",
                    string.Empty
                );
                channel.QueueBind(
                    queue: "SendNotifications",
                    exchange: "Events",
                    string.Empty
                );

                var messageId = 0;
                while(true)
                {
                    messageId++;
                    var message = $"Produce {produceId} - {messageId} > I am Nelson Nobre";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "Events",
                        routingKey: string.Empty,
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine("--> Sent {0}", message);

                    //if(messageId == 1)
                    //{
                    //    break;
                    //}

                    if(TIME_PAUSE > 0)
                    {
                        Console.WriteLine($"\tWaiting {TIME_PAUSE}ms...");
                        Thread.Sleep(TIME_PAUSE);
                    }
                }
            }

            Console.WriteLine("PRESS A KEY TO CONTINUE...");
        }
    }
}