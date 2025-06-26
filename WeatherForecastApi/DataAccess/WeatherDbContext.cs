using Microsoft.EntityFrameworkCore;
using WeatherForecastApi.Models;

namespace WeatherForecastApi.DataAccess
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
    }
}