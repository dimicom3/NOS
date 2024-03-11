using MyApiService.Models;
using Microsoft.EntityFrameworkCore;
namespace MyApiService.Data;

public class ApiDbContext : DbContext
{
    public DbSet<Temp> Temps { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
        
    }    
}