using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastApi.Controllers;
using WeatherForecastApi.DataAccess;
using WeatherForecastApi.Services;
using WeatherForecastApi.Models;
using WeatherForecastApi.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

//TODO: If time permit will work on writing unit test cases
namespace WeatherForecastApi.Tests.UnitTests
{
    public class WeatherForecastControllerTests
    {
        //TODO: Create interfaces of WeatherController and WeatherDbContext
        private readonly WeatherController _controller; 
        private readonly WeatherDbContext _dbContext;
        private readonly Mock<WeatherService> _mockWeatherService;

        public WeatherForecastControllerTests()
        {

            // Create dependencies manually
            var httpClient = new HttpClient();
            var realWeatherService = new WeatherService(httpClient); // match the constructor, can create a mock later

            // Use InMemoryDbContext as before, need to work on it
            var context = new WeatherDbContext(new DbContextOptions<WeatherDbContext>());

            // Pass real service
            var controller = new WeatherController(realWeatherService, context);
        }

        [Fact]
        public async Task WeatherForecastTest()
        {
            // Arrange
            bool Expected = true;

            // Act
            bool Actual = true;

            // Assert
            Assert.Equal(Expected, Actual);
        }


        [Fact]
        public async Task GetLocations_ReturnsListOfLocations()
        {
            // Arrange
            _dbContext.Locations.Add(new Location { Latitude = 10, Longitude = 20 });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetLocations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var locations = Assert.IsType<List<Location>>(okResult.Value);
            Assert.Single(locations);
        }

    }
}
