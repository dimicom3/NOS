using MyApiService2.Data;
using MyApiService2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiService2.Services;
using Npgsql.Replication;

namespace MyApiService2.Controllers;
[ApiController]
[Route("[controller]")]
public class SensorController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ApiDbContext _context;    
    
    public SensorController(ILogger<SensorController> logger, ApiDbContext context, IMessageService messageService){
        _context = context;    
        _messageService = messageService;
    }    
        
    [Route("get")]    
    [HttpGet]
    public async Task<IActionResult> Get()
    {     
        
        var payload = await _context.SensorData.ToListAsync();
        return Ok(payload);
    }
    [HttpPost("create/{temp}/{pressure}/{humidity}/{timeC}")]
    public async Task<IActionResult> Create(float temp, float pressure, float humidity, DateTime timeC)
    {
    // Create a new SensorData object using the route parameters
        var data = new SensorData
        {
            Temperature = temp,
            Pressure = pressure,
            Humidity = humidity,
            Time = timeC
        };
        _messageService.Enqueue(data.ToString());

        _context.SensorData.Add(data);
        await _context.SaveChangesAsync();

        return Ok("SensorData created successfully.");
    }


    [Route("post")]
    [HttpPost]
    public async Task<IActionResult> Post(SensorData data)
    {        
        var test = await _context.SensorData.AddAsync(data);
        await _context.SaveChangesAsync();
        string m = data.ToString();
        _messageService.Enqueue(m);
        return Ok(test);
    }
}