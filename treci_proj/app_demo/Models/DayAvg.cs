namespace MyApiService.Models;
public class DayAvg
{
    public int Id { get; set; }
    public float Value {get; set;}
    public DateOnly Date { get; set; }
}