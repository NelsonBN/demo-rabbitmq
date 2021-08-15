using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Demo.RabbitMQ.Events.Consumer
{
    public class Program
    {
        private const string MQ_Queue = "TestQueue2";
        private static uint _count = 0;

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
                    queue: MQ_Queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, eventArgs) => _consumerReceived(channel, eventArgs);

                channel.BasicConsume(
                    queue: MQ_Queue,
                    autoAck: false,
                    consumer: consumer
                );

                Console.WriteLine($"Consumer > {consumerId} runing...");
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

                if(_count % 2 == 0)
                {
                    throw new Exception($"ERROR: {message}"); // To test
                }

                Console.WriteLine($"{_count} > Received {message}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch(Exception exception)
            {
                channel.BasicNack(eventArgs.DeliveryTag, false, false);
                Console.WriteLine($"{_count} > Exception {exception.Message}");
            }

            System.Threading.Thread.Sleep(4000);
            Console.WriteLine("...");
            _count++;
        }
    }
}