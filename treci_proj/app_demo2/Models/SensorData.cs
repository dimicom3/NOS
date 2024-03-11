using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiService2.Models;

public class SensorData{

    public int ID {get; set;}
    public float Temp{get; set;}

    public float Pressure{get; set;}

    public float Humidity{get; set;}

    public float TimeC{get; set;}
    public override string ToString()
    {
        return $"ID: {ID}, Temperature: {Temp}, Pressure: {Pressure}, Humidity: {Humidity}, TimeC: {TimeC}";
    }
}