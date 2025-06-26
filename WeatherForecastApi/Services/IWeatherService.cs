using WeatherForecastApi.Models;

namespace WeatherForecastApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherForecast?> GetForecastAsync(double lat, double lon);
    }
}
