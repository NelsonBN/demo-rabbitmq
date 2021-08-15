﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Demo.RabbitMQ.Fanout.Consumer.SaveEvents
{
    public class Program
    {
        private const string MQ_Queue = "SaveEvents";

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

                Console.WriteLine($"Consumer {MQ_Queue} > PRESS A KEY TO CONTINUE...");
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

                Console.WriteLine($"Consume {MQ_Queue} >>> Received {message}");
                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch(Exception exception)
            {
                channel.BasicNack(eventArgs.DeliveryTag, false, true);
                Console.WriteLine($"Consume {MQ_Queue} >>> Exception {exception.Message}");

                Thread.Sleep(100);
            }
        }
    }
}