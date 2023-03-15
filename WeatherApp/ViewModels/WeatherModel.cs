using WeatherApp.Models;

namespace WeatherApp.ViewModels;

public class WeatherViewModel
{
    public string City { get; set; }
    
    public List<WeatherDto> Weather { get; set; }
}