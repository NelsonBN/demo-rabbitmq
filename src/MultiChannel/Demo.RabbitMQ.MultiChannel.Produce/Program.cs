using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.RabbitMQ.MultiChannel.Produce
{
    public class Program
    {
        private const string MQ_QUEUE = "TestQueue";
        private const int TIME_PAUSE = 20;

        static void Main(string[] _)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            {
                _publishMessages(
                    connection.CreateModel() // Channel 1
                );
                _publishMessages(
                    connection.CreateModel() // Channel 2
                );

                Console.WriteLine("PRODUCE > PRESS A KEY TO CONTINUE...");
                Console.ReadLine();
            }
        }

        private static void _publishMessages(IModel channel)
        {
            Task.Factory.StartNew(() =>
            {
                var channelId = Guid.NewGuid();

                channel.QueueDeclare(
                    queue: MQ_QUEUE,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var messageId = 0;
                while(true)
                {
                    messageId++;
                    var message = $"Channel {channelId} - {messageId} > I am Nelson Nobre";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: MQ_QUEUE,
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine($"--> {message}");

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
            });
        }
    }
}