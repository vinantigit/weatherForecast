/// <summary>
/// Implements the <see cref="IWeatherService"/> interface to fetch current weather forecast data
/// from the Open-Meteo API using geographic coordinates.
/// Parses JSON responses and returns a simplified <see cref="WeatherForecast"/> model.
/// </summary>
/// 

namespace WeatherForecastApi.Services
{
    using System.Text.Json;
    using WeatherForecastApi.Models;
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _http;

        public WeatherService(HttpClient http)
        {
            _http = http;
        }

        public async Task<WeatherForecast?> GetForecastAsync(double lat, double lon)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";

            var response = await _http.GetFromJsonAsync<JsonElement>(url);
            if (response.TryGetProperty("current_weather", out var current))
            {
                return new WeatherForecast
                {
                    Temperature = current.GetProperty("temperature").GetDouble(),
                    Summary = "Live data"
                };
            }

            return null;
        }
    }
}