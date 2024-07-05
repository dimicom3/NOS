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
builder.Services.AddSingleton<RabbitMQService>(); 
// builder.Services.AddSingleton<RabbitMQService>();
var app = builder.Build();
using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


// Task.Delay(20000).Wait();
// Console.WriteLine("Consuming Queue Now");

// ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
// factory.UserName = "guest";
// factory.Password = "guest";
// IConnection connRabbit = factory.CreateConnection();
// IModel channel = connRabbit.CreateModel();


// channel.QueueDeclare(queue: "hello",
//                         durable: false,
//                         exclusive: false,
//                         autoDelete: false,
//                         arguments: null);
// EventingBasicConsumer consumerHello = new EventingBasicConsumer(channel);
// initConsumerHello(consumerHello);
// channel.BasicConsume(queue: "hello",
//                         autoAck: true,
//                         consumer: consumerHello);

// channel.QueueDeclare(queue: "dailyavg",
//                         durable: false,
//                         exclusive: false,
//                         autoDelete: false,
//                         arguments: null);
// EventingBasicConsumer consumerAvg = new EventingBasicConsumer(channel);
// initConsumerAvg(consumerAvg);
// channel.BasicConsume(queue: "dailyavg",
//                         autoAck: true,
//                         consumer: consumerAvg);

// if (!context.Database.CanConnect())
// {
    Console.WriteLine("DATABASE MIGRATE STARTS");
    context.Database.Migrate();
    Console.WriteLine("DATABASE MIGRATE END");
// }
// else
// {
//     Console.WriteLine("Database already exists. Skipping database migration.");
// }
var rabbitMqService = app.Services.GetRequiredService<RabbitMQService>();
app.Run();



// void initConsumerHello(EventingBasicConsumer consumer) {
        
//     consumer.Received += (model, ea) =>
//     {
//         var body = ea.Body.Span;
//         var message = Encoding.UTF8.GetString(body);
//         Console.WriteLine(" [x] Received from Rabbit: {0}", message);

//         string[] pairs = message.Split(','); 

//         int id = 0;
//         float temperature = 0;
//         float pressure = 0;
//         float humidity = 0;
//         DateTime timeC = DateTime.Now;

//         foreach (var pair in pairs)
//         {
//             string[] keyValue = pair.Trim().Split(':');

//             if (keyValue.Length == 2)
//             {
//                 string key = keyValue[0].Trim();
//                 string value = keyValue[1].Trim();

//                 if (key == "ID")
//                     id = int.Parse(value);
//                 else if (key == "Temperature")
//                     temperature = float.Parse(value);
//                 else if (key == "Pressure")
//                     pressure = float.Parse(value);
//                 else if (key == "Humidity")
//                     humidity = float.Parse(value);
//                 else if (key == "TimeC")
//                     timeC = DateTime.Parse(value);
//             }
//         }

//             var data = new Temp {Value = temperature, Time = timeC };
//             context.Temps.Add(data);
//             context.SaveChanges();
//     };
// }

// void initConsumerAvg(EventingBasicConsumer consumer) {
        
//     consumer.Received += (model, ea) =>
//     {
//         var body = ea.Body.Span;
//         var message = Encoding.UTF8.GetString(body);
//         Console.WriteLine(" [x] Received from Rabbit: {0}", message);

//         float temperature = 0;

//         DateOnly timeC = DateOnly.FromDateTime(DateTime.Now);

//         string[] keyValue = message.Trim().Split(':');

//         if (keyValue.Length == 2)
//         {
//             string key = keyValue[0].Trim();
//             string value = keyValue[1].Trim();
//             temperature = float.Parse(value);
    
//         }
        
//         var data = new DayAvg {Value = temperature, Date = timeC};
//         context.DayAvgs.Add(data);
//         context.SaveChanges();
//     };
// }
