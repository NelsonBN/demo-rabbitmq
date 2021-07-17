# RabbitMQ


## Documentation
The RabbitMQ is an open source message broker developed in Erlang. It supports multiple messaging protocols. RabbitMQ can be deployed in distributed and federated configurations to meet high-scale, high-availability requirements.


### Supported protocols
- AMQP (Advanced Message Queuing Protocol) *More common*
- MQTT
- STOMP
- HTTP and WebSockets

### Performance
RabbitMQ by default stores messages in memory and uses TCP-based protocols, so RabbitMQ is a very fast broker to pass messages

### Communication flow
![Communication flow](https://i.imgur.com/GzNKKPy.png "Communication flow")


## Demo

### Technologies implemented:
- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [NuGet Package RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/)
- [Image docker for RabbitMQ broker](https://hub.docker.com/_/rabbitmq)

#### Run RabbitMQ broker (Using docker)
```bash
docker run -d --hostname rabbitmqservice --name rabbitmqservice -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```
> The image "rabbitmq:3-management" has a web interface to management the RabbitMQ. Can access by http://localhost:15672/ username "guest" and password "guest"


## Reference links
* [Official site](https://rabbitmq.com/)
* [RabbitMQ Performance Measurements](https://blog.rabbitmq.com/posts/2012/04/rabbitmq-performance-measurements-part-2)


## Contribution

*Help me to help others*

## LICENSE

[MIT](https://github.com/NelsonBN/Demo-Class-vs-Record-vs-Struct/blob/main/LICENSE)