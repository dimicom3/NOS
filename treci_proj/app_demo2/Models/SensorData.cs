using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiService2.Models;

public class SensorData{

    public int ID {get; set;}
    public float Temperature{get; set;}

    public float Pressure{get; set;}

    public float Humidity{get; set;}

    public DateTime Time{get; set;}
    public override string ToString()
    {
        return $"ID: {ID}, Temperature: {Temperature}, Pressure: {Pressure}, Humidity: {Humidity}, TimeC: {Time}";
    }
}