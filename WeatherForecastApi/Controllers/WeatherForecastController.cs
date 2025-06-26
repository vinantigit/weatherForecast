using Microsoft.AspNetCore.Mvc;
using WeatherForecastApi.Models;
using WeatherForecastApi.Data;
using WeatherForecastApi.Services;

namespace WeatherForecastApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/weather
        [HttpGet]
        public ActionResult<IEnumerable<Location>> GetLocations()
        {
            return Ok(InMemoryStore.Locations);
        }

        // POST: api/weather
        [HttpPost]
        public ActionResult<Location> AddLocation([FromBody] Location location)
        {
            location.Id = InMemoryStore.Locations.Count > 0
                ? InMemoryStore.Locations.Max(x => x.Id) + 1
                : 1;

            InMemoryStore.Locations.Add(location);
            return CreatedAtAction(nameof(GetLocations), new { id = location.Id }, location);
        }

        // DELETE: api/weather/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteLocation(int id)
        {
            var location = InMemoryStore.Locations.FirstOrDefault(x => x.Id == id);
            if (location == null) return NotFound();

            InMemoryStore.Locations.Remove(location);
            return NoContent();
        }

        // GET: api/weather/forecast/{id}
        [HttpGet("forecast/{id}")]
        public async Task<ActionResult<WeatherForecast>> GetForecast(int id)
        {
            var location = InMemoryStore.Locations.FirstOrDefault(x => x.Id == id);
            if (location == null) return NotFound();

            var forecast = await _weatherService.GetForecastAsync(location.Latitude, location.Longitude);
            return forecast != null ? Ok(forecast) : StatusCode(503, "Unable to fetch forecast.");
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }


}
