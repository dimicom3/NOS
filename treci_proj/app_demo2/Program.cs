using MyApiService2.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using MyApiService2.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using System.Timers;
[assembly: ApiController]


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(conn));

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.AddSingleton<IMessageService, MessageService>();


var app = builder.Build();
using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();

var messageService = serviceScope.ServiceProvider.GetRequiredService<IMessageService>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (!app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

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

SetTimer(context, messageService);

app.Run();


void SetTimer(ApiDbContext context, IMessageService messageService)
{
    System.Timers.Timer timer = new System.Timers.Timer(10*1000);//24 * 60 * 60 * 1000); // 24 hours in milliseconds
    timer.Elapsed += (sender, e) => OnTimedEvent(context, messageService);
    timer.AutoReset = true;
    timer.Enabled = true;
}

void OnTimedEvent(ApiDbContext context, IMessageService messageService)
{
    double averageTemperature = GetDailyAverageTemperature(context);
    SendMessage(averageTemperature, messageService);
}

double GetDailyAverageTemperature(ApiDbContext context)
{
    var today = DateTime.Today;
    var sensorData = context.SensorData
                            .Where(sd => sd.Time.Date == today)
                            .ToList();

    if (sensorData.Count == 0)
    {
        return 0; 
    }

    double totalTemperature = sensorData.Sum(sd => sd.Temperature);
    return totalTemperature / sensorData.Count;
}

void SendMessage(double averageTemperature, IMessageService messageService)
{
    string message = $"averageTemperature: {averageTemperature}";
    messageService.EnqueueAvg(message);
}