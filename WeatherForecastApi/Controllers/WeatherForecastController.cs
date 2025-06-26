using Microsoft.AspNetCore.Mvc;
using WeatherForecastApi.Models;
using WeatherForecastApi.Services;
using WeatherForecastApi.DataAccess;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var locations = await _context.Locations.ToListAsync();

                if (locations == null || !locations.Any())
                {
                    _logger.LogInformation("No locations found in the database.");
                    return NotFound("No locations available.");
                }

                _logger.LogInformation("Retrieved {Count} locations from the database.", locations.Count);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving locations.");
                return StatusCode(500, "An unexpected error occurred while fetching locations.");
            }
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
            if (id <= 0)
            {
                _logger.LogWarning("Invalid delete request received for ID: {Id}", id);
                return BadRequest("Invalid location ID.");
            }

            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                {
                    _logger.LogInformation("Location ID {Id} not found for deletion.", id);
                    return NotFound($"Location with ID {id} not found.");
                }

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted Location ID {Id}", id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting Location ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the location.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during delete operation for ID {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // GET: api/weather/forecast/{id}
        [HttpGet("forecast/{id}")]
        public async Task<ActionResult<WeatherForecast>> GetForecast(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid location ID received: {Id}", id);
                return BadRequest("Invalid location ID.");
            }

            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                {
                    _logger.LogInformation("Location with ID {Id} not found.", id);
                    return NotFound($"Location with ID {id} not found.");
                }

                var forecast = await _weatherService.GetForecastAsync(location.Latitude, location.Longitude);
                if (forecast == null)
                {
                    _logger.LogWarning("Forecast unavailable for Location ID {Id} (Lat: {Lat}, Long: {Long})",
                        id, location.Latitude, location.Longitude);
                    return StatusCode(503, "Unable to fetch forecast.");
                }

                return Ok(forecast);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Weather service HTTP error for Location ID {Id}", id);
                return StatusCode(503, "Weather service is unavailable.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving forecast for Location ID {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
