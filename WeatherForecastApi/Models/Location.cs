using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApi.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
