/// <summary>
/// Defines a contract for retrieving weather forecast data based on geographic coordinates.
/// Implementations should fetch forecast information for the specified latitude and longitude.
/// </summary>
/// <param name="lat">The latitude of the location.</param>
/// <param name="lon">The longitude of the location.</param>
/// <returns>A <see cref="WeatherForecast"/> instance if available; otherwise, null.</returns>

namespace WeatherForecastApi.Services
{
    using WeatherForecastApi.Models;
    public interface IWeatherService
    {
        Task<WeatherForecast?> GetForecastAsync(double lat, double lon);
    }
}
