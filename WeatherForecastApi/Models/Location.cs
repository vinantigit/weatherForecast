/// <summary>
/// Represents a geographic location with latitude and longitude coordinates.
/// Used for storing and retrieving location data within the Weather Forecast API.
/// Includes validation attributes to ensure data integrity.
/// </summary>

namespace WeatherForecastApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public double Longitude { get; set; }

    }
}
