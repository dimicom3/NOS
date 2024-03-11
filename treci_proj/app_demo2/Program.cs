using MyApiService2.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using MyApiService2.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(conn));
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddSingleton<IMessageService, MessageService>();


var app = builder.Build();
using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();

if (!app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

Console.WriteLine("DATABASE MIGRATE STARTS");
context.Database.Migrate();
Console.WriteLine("DATABASE MIGRATE END");

app.Run();
