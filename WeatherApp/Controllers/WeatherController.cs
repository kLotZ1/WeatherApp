using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers;

[Authorize(Policy = "CanGetWeather")]
public class WeatherController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        var vm = new WeatherViewModel();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string city, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("WeatherApiClient");
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/WeatherForecast");

        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        using (var stream = await response.Content.ReadAsStreamAsync(cancellationToken))
        {
            var forecasts =
                await JsonSerializer.DeserializeAsync<List<WeatherDto>>(stream, cancellationToken: cancellationToken);
            return View(new WeatherViewModel
            {
                City = city,
                Weather = forecasts
            });
        }
    }
}