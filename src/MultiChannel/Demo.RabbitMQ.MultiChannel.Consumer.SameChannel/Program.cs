using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.MultiChannel.Consumer.SameChannel
{
    public class Program
    {
        private const string MQ_Queue = "TestQueue";

        static void Main(string[] _)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: MQ_Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    for(var consumerId = 1; consumerId <= 10; consumerId++)
                    {
                        _createConsumer(channel, consumerId);
                    }

                    Console.WriteLine("CONSUMER > PRESS A KEY TO CONTINUE...");
                    Console.ReadLine();
                }
            }
        }

        private static void _createConsumer(IModel channel, int consumerId)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
            {
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"ConsumerId {consumerId} > Received {message}");
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch(Exception exception)
                {
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    Console.WriteLine($"ConsumerId {consumerId} > Exception {exception.Message}");

                    System.Threading.Thread.Sleep(100);
                }
            };

            channel.BasicConsume(
                queue: MQ_Queue,
                autoAck: false,
                consumer: consumer
            );
        }
    }
}