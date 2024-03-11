using MyApiService.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MyApiService.Services;
using MyApiService.Models;
using System.Runtime.CompilerServices;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(conn));
builder.Services.AddControllers();
// builder.Services.AddSingleton<RabbitMQService>();
var app = builder.Build();
using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


Task.Delay(20000).Wait();
Console.WriteLine("Consuming Queue Now");

ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
factory.UserName = "guest";
factory.Password = "guest";
IConnection connRabbit = factory.CreateConnection();
IModel channel = connRabbit.CreateModel();
channel.QueueDeclare(queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.Span;
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received from Rabbit: {0}", message);

    string[] pairs = message.Split(','); 

    int id = 0;
    float temperature = 0;
    float pressure = 0;
    float humidity = 0;
    float timeC = 0;

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
                timeC = float.Parse(value);
        }
    }

    var data = new Temp { Id = id, Value = temperature, TimeC = timeC };
    context.Temps.Add(data);
    context.SaveChanges();
};
channel.BasicConsume(queue: "hello",
                        autoAck: true,
                        consumer: consumer);

app.MapControllers();


Console.WriteLine("DATABASE MIGRATE STARTS");
context.Database.Migrate();
Console.WriteLine("DATABASE MIGRATE END");

app.Run();

