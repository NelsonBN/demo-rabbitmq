using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.Basic.Consumer.AutoAck
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
                consumer.Received += (sender, eventArgs) => _consumerReceived(consumerId, eventArgs);

                channel.BasicConsume(
                    queue: "myqueue",
                    autoAck: true,
                    consumer: consumer
                );

                Console.WriteLine($"ConsumerAPP {consumerId} runing...");
                Console.ReadLine();
            }
        }

        private static void _consumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var consumerId = (Guid)sender;
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                //throw new Exception("TEST..."); // To test

                Console.WriteLine($"Consume {consumerId} >>> Received {message}");
            }
            catch(Exception exception)
            {
                Console.WriteLine($"Consume {consumerId} >>> Exception {exception.Message}");
            }
        }
    }
}