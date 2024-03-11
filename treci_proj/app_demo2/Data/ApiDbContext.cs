using MyApiService2.Models;
using Microsoft.EntityFrameworkCore;
namespace MyApiService2.Data;

public class ApiDbContext : DbContext
{
    public DbSet<SensorData> SensorData {get;set;}

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
        
    }    
}