using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherForecastApi.Models;

namespace WeatherForecastApi.Services
{
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