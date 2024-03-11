using System.Text;
using System.Transactions;
using MyApiService.Data;
using MyApiService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyApiService.Services;

public class RabbitMQService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
        factory.UserName = "guest";
        factory.Password = "guest";
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "hello",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.Span;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received from Rabbit: {0}", message);

            // Delegate the message handling to another method or class
            HandleMessageAsync(message);
        };

        _channel.BasicConsume(queue: "hello",
                              autoAck: true,
                              consumer: consumer);
    }

    private async Task HandleMessageAsync(string message)
    {
        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        {
            // Use the injected DbContext to interact with the database
            using (var dbContext = _serviceProvider.GetRequiredService<ApiDbContext>())
            {
                Console.WriteLine(message);
                dbContext.Temps.Add(new Temp());
                await dbContext.SaveChangesAsync();
            }

            scope.Complete();
        }
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}