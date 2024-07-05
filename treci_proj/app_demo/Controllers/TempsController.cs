using MyApiService.Data;
using MyApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class TempsController : ControllerBase
{
    private readonly ApiDbContext _context;    
    
    public TempsController(ApiDbContext context)
    {
        
        _context = context;  
        
    }    
        
    [Route("get")]    
    [HttpGet]
    public async Task<IActionResult> Get()
    {       
        
        var result = await _context.Temps.ToListAsync();
        return Ok(result);
    }
    [Route("getDailyAvg")]    
    [HttpGet]
    public async Task<IActionResult> getDailyAvg()
    {       
        
        var result = await _context.DayAvgs.ToListAsync();
        return Ok(result);
    }
}