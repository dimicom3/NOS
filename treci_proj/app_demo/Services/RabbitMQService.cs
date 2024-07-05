using System.Text;
using MyApiService.Data;
using MyApiService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IModel _channelHello;
    private readonly IModel _channelAvg;
    private readonly ConnectionFactory _factory;
    private IConnection _connRabbit;

    private readonly int _maxRetries = 5;
    private readonly int _delayMilliseconds = 5000; // 5 seconds

    public RabbitMQService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672, UserName = "guest", Password = "guest" };

        TryConnect();

        _channelHello = _connRabbit.CreateModel();
        _channelHello.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumerHello = new EventingBasicConsumer(_channelHello);
        consumerHello.Received += HandleHelloMessage;
        _channelHello.BasicConsume(queue: "hello", autoAck: true, consumer: consumerHello);

        _channelAvg = _connRabbit.CreateModel();
        _channelAvg.QueueDeclare(queue: "dailyavg", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumerAvg = new EventingBasicConsumer(_channelAvg);
        consumerAvg.Received += HandleAvgMessage;
        _channelAvg.BasicConsume(queue: "dailyavg", autoAck: true, consumer: consumerAvg);
    }

    private void TryConnect()
    {
        int attempts = 0;
        while (attempts < _maxRetries)
        {
            try
            {
                _connRabbit = _factory.CreateConnection();
                Console.WriteLine("Connected to RabbitMQ");
                return;
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
            {
                attempts++;
                Console.WriteLine($"Attempt {attempts} failed. Retrying in {_delayMilliseconds / 1000} seconds...");
                Task.Delay(_delayMilliseconds).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        throw new Exception("Could not connect to RabbitMQ after several attempts.");
    }

    private void HandleHelloMessage(object model, BasicDeliverEventArgs ea)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        var body = ea.Body.Span;
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received from Rabbit: {0}", message);

        string[] pairs = message.Split(',');

        int id = 0;
        float temperature = 0;
        float pressure = 0;
        float humidity = 0;
        DateTime timeC = DateTime.Now;

        foreach (var pair in pairs)
        {
            string[] keyValue = pair.Trim().Split(':');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                if (key == "ID")
                    id = int.Parse(value);
                else if (key == "Temperature")
                    temperature = float.Parse(value);
                else if (key == "Pressure")
                    pressure = float.Parse(value);
                else if (key == "Humidity")
                    humidity = float.Parse(value);
                else if (key == "TimeC")
                    timeC = DateTime.Parse(value);
            }
        }

        var data = new Temp { Value = temperature, Time = timeC };
        context.Temps.Add(data);
        context.SaveChanges();
    }

    private void HandleAvgMessage(object model, BasicDeliverEventArgs ea)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        var body = ea.Body.Span;
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received from Rabbit: {0}", message);

        float temperature = 0;
        DateOnly timeC = DateOnly.FromDateTime(DateTime.Now);

        string[] keyValue = message.Trim().Split(':');

        if (keyValue.Length == 2)
        {
            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();
            temperature = float.Parse(value);
        }

        var data = new DayAvg { Value = temperature, Date = timeC };
        context.DayAvgs.Add(data);
        context.SaveChanges();
    }
}
