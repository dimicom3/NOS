using System;
using System.Text;
using RabbitMQ.Client;

namespace MyApiService2.Services
{
    // define interface and service
    public interface IMessageService
    {
        bool Enqueue(string message);
        bool EnqueueAvg(string message);
    }

    public class MessageService : IMessageService
    {
        ConnectionFactory _factory;
        IConnection _conn;
        IModel _channel;
        private readonly int _maxRetries = 5;
        private readonly int _delayMilliseconds = 5000; // 5 seconds
        public MessageService()
        {
            Console.WriteLine("about to connect to rabbit");

            int attempts = 0;
            while (attempts < _maxRetries)
            {
                try{
                    _factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
                    _factory.UserName = "guest";
                    _factory.Password = "guest";
                    _conn = _factory.CreateConnection();
                    _channel = _conn.CreateModel();
                    
                    _channel.QueueDeclare(queue: "hello",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                    _channel.QueueDeclare(queue: "dailyavg",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null); 
                    return;           
                }catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException){
                    attempts++;
                    Console.WriteLine($"Attempt {attempts} failed. Retrying in {_delayMilliseconds / 1000} seconds...");
                    Task.Delay(_delayMilliseconds).Wait();
                }
                catch (Exception ex){
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    throw;
                }       
            }   
            throw new Exception("Could not connect to RabbitMQ after several attempts.");             
        }
        
        public bool Enqueue(string messageString)
        {
            var body = Encoding.UTF8.GetBytes("server processed " + messageString);
            _channel.BasicPublish(exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);
            Console.WriteLine(" [x] Published {0} to RabbitMQ", messageString);
            return true;
        }
        public bool EnqueueAvg(string messageString)
        {
            var body = Encoding.UTF8.GetBytes("server processed " + messageString);
            _channel.BasicPublish(exchange: "",
                                routingKey: "dailyavg",
                                basicProperties: null,
                                body: body);
            Console.WriteLine(" [x] Published {0} to RabbitMQ", messageString);
            return true;
        }
        
    }
}