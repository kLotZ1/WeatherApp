using System.Text.Json.Serialization;

namespace WeatherApp.Models;

[Serializable]
public class WeatherDto
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("temperatureC")]
    public int TemperatureC { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
}