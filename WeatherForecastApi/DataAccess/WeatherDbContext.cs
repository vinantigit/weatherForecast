/// <summary>
/// Represents the Entity Framework Core database context for the Weather Forecast API.
/// Provides access to the <see cref="Location"/> data set used for storing and querying location entities.
/// </summary>

namespace WeatherForecastApi.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using WeatherForecastApi.Models;

    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
    }
}