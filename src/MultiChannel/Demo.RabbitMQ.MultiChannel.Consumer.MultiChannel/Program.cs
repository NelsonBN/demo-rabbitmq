using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.MultiChannel.Consumer.MultiChannel
{
    public class Program
    {
        private const string MQ_QUEUE = "TestQueue";

        static void Main(string[] _)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            {
                for(var count = 1; count <= 10; count++)
                {
                    _createConsumer(connection.CreateModel(), count);
                }

                Console.WriteLine("CONSUMER > PRESS A KEY TO CONTINUE...");
                Console.ReadLine();
            }
        }

        private static void _createConsumer(IModel channel, int channelId)
        {
            channel.QueueDeclare(
                queue: MQ_QUEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"ChannelId {channelId} > Received {message}");
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch(Exception exception)
                {
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    Console.WriteLine($"ChannelId {channelId} > Exception {exception.Message}");

                    System.Threading.Thread.Sleep(100);
                }
            };

            channel.BasicConsume(
                queue: MQ_QUEUE,
                autoAck: false,
                consumer: consumer
            );
        }
    }
}