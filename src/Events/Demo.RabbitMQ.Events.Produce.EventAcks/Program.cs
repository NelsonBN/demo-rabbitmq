using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.Events.Produce.EventAcks
{
    public class Program
    {
        private const string MQ_Queue = "TestQueue2";
        private const int TIME_PAUSE = 4000;

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
                channel.ConfirmSelect();

                channel.QueueDeclare(
                    queue: MQ_Queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.BasicAcks += _channelBasicAcks;

                channel.WaitForConfirms(TimeSpan.FromSeconds(2));

                var messageId = 0;
                while(true)
                {
                    messageId++;
                    var message = $"Produce {produceId} - {messageId} > I am Nelson Nobre";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: MQ_Queue,
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

            Console.WriteLine("PRODUCE > PRESS A KEY TO CONTINUE...");
        }

        private static void _channelBasicAcks(object sender, global::RabbitMQ.Client.Events.BasicAckEventArgs eventArgs)
        {
            Console.WriteLine($"Event > Acks > DeliveryTag: {eventArgs.DeliveryTag}");
        }
    }
}