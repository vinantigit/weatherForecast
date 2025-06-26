/// <summary>
/// Represents a weather forecast result, including temperature and a textual summary.
/// Used to return forecast data to API consumers.
/// </summary>
/// 
namespace WeatherForecastApi.Models
{
    public class WeatherForecast
    {
        public string? Summary { get; set; }
        
        public double Temperature { get; set; }

    }
}
