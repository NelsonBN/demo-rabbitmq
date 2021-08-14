using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.Basic.Consumer.ManualAck
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
                consumer.Received += (sender, eventArgs) => _consumerReceived(channel, eventArgs);

                channel.BasicConsume(
                    queue: "myqueue",
                    autoAck: false,
                    consumer: consumer
                );

                Console.WriteLine($"ConsumerAPP {consumerId} runing...");
                Console.ReadLine();
            }
        }

        private static void _consumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var channel = (IModel)sender;

            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                //throw new Exception("TEST..."); // To test

                Console.WriteLine($"Received {message}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch(Exception exception)
            {
                channel.BasicNack(eventArgs.DeliveryTag, false, true);
                Console.WriteLine($"Exception {exception.Message}");

                System.Threading.Thread.Sleep(100);
            }
        }
    }
}