 RabbitMQ



## Technologies implemented:
- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [NuGet Package RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/)
- [Image docker for RabbitMQ broker](https://hub.docker.com/_/rabbitmq)



## Documentation
The RabbitMQ is an open source message broker developed in Erlang. It supports multiple messaging protocols. RabbitMQ can be deployed in distributed and federated configurations to meet high-scale, high-availability requirements.

### Supported protocols
- AMQP (Advanced Message Queuing Protocol) *More common*
- MQTT
- STOMP
- HTTP and WebSockets

### Performance
RabbitMQ by default stores messages in memory and uses TCP-based protocols, so RabbitMQ is a very fast broker to pass messages

### RabbitMQ

#### RabbitMQ CLI
Enter in docker
```bash
docker exec -it rabbitmq-server bash
```
```bash
rabbitmqctl ....
```

##### Plugins
```bash
rabbitmq-plugins ....
```
```bash
rabbitmq-plugins list
```



## Demo

### Run RabbitMQ broker (Using docker)
First option, pull basic server and install web manager
```bash
docker run -d --hostname rabbitmq-service --name rabbitmq-service -p 15672:15672 -p 5672:5672 rabbitmq:3
docker exec -it rabbitmq-server bash
rabbitmq-plugins enable rabbitmq_management
rabbitmq-plugins list
```
**Or**
Second option, pull the server with the web management  installed
```bash
docker run -d --hostname rabbitmq-service --name rabbitmq-service -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```
> The image "rabbitmq:3-management" has a web interface to management the RabbitMQ. Can access by http://localhost:15672/ username "guest" and password "guest"

- **Port: 4369:** epmd, a helper discovery daemon used by RabbitMQ nodes and CLI tools;
- **Port: 5672:** AMQP listener;
- **Port: 25672:** Clustering listener;
- **Port: 15672:** HTTP UI to management RabbitMQ;
- **Port: 15692:** HTTP Prometheus host;

### Demo - Basic
Uncomment the exception and check the message has been removed from the queue

#### Communication flow
![Communication flow](https://www.rabbitmq.com/img/tutorials/intro/hello-world-example-routing.png "Communication flow")


#### Demo.RabbitMQ.Basic.Consumer.AutoAck
```csharp
..
var body = eventArgs.Body.ToArray();
var message = Encoding.UTF8.GetString(body);

throw new Exception("TEST..."); // To test

Console.WriteLine($"Consume {consumerId} >>> Received {message}");
...
```


#### Demo.RabbitMQ.Basic.Consumer.ManualAck (RECOMMENDED)
_Difference_
```csharp
channel.BasicConsume(
    ..
    autoAck: true,
    ..
);

try
{
    ..
    channel.BasicAck(eventArgs.DeliveryTag, false);
}
catch(Exception exception)
{
    channel.BasicNack(eventArgs.DeliveryTag, false, true);
    ..
}
```


### Demo - Fanout

#### Communication flow
![Communication flow](https://www.rabbitmq.com/img/tutorials/intro/exchange-fanout.png "Communication flow")

#### Demo.RabbitMQ.Fanout.Produce
```csharp
// Create exchange
channel.ExchangeDeclare(exchange: "Events", type: "fanout");

// Create queues
channel.QueueDeclare(queue: "SaveEvents", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "SendNotifications", durable: false, exclusive: false, autoDelete: false, arguments: null);

// Attach queue to exchange
channel.QueueBind(queue: "SaveEvents", exchange: "Events", string.Empty);
channel.QueueBind(queue: "SendNotifications", exchange: "Events", string.Empty);
```


### Demo - Direct

#### Communication flow
![Communication flow](https://www.rabbitmq.com/img/tutorials/intro/exchange-direct.png "Communication flow")

#### Demo.RabbitMQ.Direct.Produce
```csharp
// Create exchange
channel.ExchangeDeclare(exchange: "Products", type: "direct");

// Create queues
channel.QueueDeclare(queue: "Cart", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "Notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);

// Attach queue to exchange
channel.QueueBind(queue: "Cart", exchange: "Products", routingKey: "Save");
channel.QueueBind(queue: "Notifications", exchange: "Products", routingKey: "Save");
channel.QueueBind(queue: "Notifications", exchange: "Products", routingKey: "Notify");
```


### Demo - Topic

#### Communication flow
![Communication flow](https://chathurangat.files.wordpress.com/2017/11/screen-shot-2017-11-12-at-4-56-34-am.png "Communication flow")

#### Demo.RabbitMQ.Topic.Produce
```csharp
 // Create exchange
channel.ExchangeDeclare(exchange: "Notifications", type: "topic");

// Create queues
channel.QueueDeclare(queue: "SMS", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "Push", durable: false, exclusive: false, autoDelete: false, arguments: null);

// Attach queue to exchange
channel.QueueBind(queue: "SMS", exchange: "Notifications", routingKey: "Products.#");
channel.QueueBind(queue: "Push", exchange: "Notifications", routingKey: "*.Promotions.*");
channel.QueueBind(queue: "Push", exchange: "Notifications", routingKey: "Stock.#");
```



## Reference links
* [.NET/C# Client API Guide](https://www.rabbitmq.com/dotnet-api-guide.html)
* [RabbitMQ Performance Measurements](https://blog.rabbitmq.com/posts/2012/04/rabbitmq-performance-measurements-part-2)
* [RabbitMQ container](https://hub.docker.com/_/rabbitmq)
* [RabbitMQ Simulator](http://tryrabbitmq.com/)



## Contribution

*Help me to help others*



## LICENSE

[MIT](https://github.com/NelsonBN/demo-rabbitmq/blob/main/LICENSE)