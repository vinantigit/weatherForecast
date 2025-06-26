using Microsoft.AspNetCore.Mvc;
using WeatherForecastApi.Models;
using WeatherForecastApi.Data;
using WeatherForecastApi.Services;
using WeatherForecastApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WeatherForecastApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(WeatherService weatherService, WeatherDbContext context, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _context = context;
            _logger = logger;

        }


        // GET: api/weather
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            return Ok(locations);
        }

        // POST: api/weather
        [HttpPost]
        public async Task<ActionResult<Location>> AddLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.Locations.AddAsync(location);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetLocations), new { id = location.Id }, location);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while saving the location.");
                return StatusCode(500, "An error occurred while saving the location.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing request");
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        // DELETE: api/weather/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return NotFound();

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/weather/forecast/{id}
        [HttpGet("forecast/{id}")]
        public async Task<ActionResult<WeatherForecast>> GetForecast(int id)
        {
            var location = await _context.Locations.FindAsync(id);
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
