/// <summary>
/// The WeatherController handles API requests related to weather locations and forecasts.
/// It provides endpoints to:
/// - Retrieve all locations from the database
/// - Add a new location
/// - Delete an existing location
/// - Fetch real-time weather forecast for a specified location
/// Uses dependency-injected services for database access, logging, and forecast retrieval.
/// </summary>

namespace WeatherForecastApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using WeatherForecastApi.Models;
    using WeatherForecastApi.Services;
    using WeatherForecastApi.DataAccess;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController"/> class,
        /// injecting services for weather forecasting, database access, and logging.
        /// </summary>
        /// <param name="weatherService">Service used to retrieve weather forecast data.</param>
        /// <param name="context">Entity Framework Core database context for managing location data.</param>
        /// <param name="logger">Logger instance for capturing diagnostic information and errors.</param>
        public WeatherController(WeatherService weatherService, WeatherDbContext context, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _context = context;
            _logger = logger;

        }

        /// <summary>
        /// Retrieves all available locations from the database.
        /// Returns a list of <see cref="Location"/> objects or a relevant error response
        /// based on the outcome of the database query.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of locations if successful;  
        /// 404 Not Found if no locations exist;  
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
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

        /// <summary>
        /// Adds a new <see cref="Location"/> to the database.
        /// Validates the incoming request body and persists the entity if valid.
        /// </summary>
        /// <param name="location">The location object to be created, provided in the request body.</param>
        /// <returns>
        /// 201 Created with the newly added location if successful;  
        /// 400 Bad Request if model validation fails;  
        /// 500 Internal Server Error for database or unexpected exceptions.
        /// </returns>
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

        /// <summary>
        /// Deletes a <see cref="Location"/> entry from the database by its ID.
        /// Validates the request, checks for existence, and removes the location if found.
        /// </summary>
        /// <param name="id">The unique identifier of the location to be deleted.</param>
        /// <returns>
        /// 204 No Content if deletion succeeds;  
        /// 400 Bad Request for invalid IDs;  
        /// 404 Not Found if the location does not exist;  
        /// 500 Internal Server Error for database or unexpected failures.
        /// </returns>
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

        /// <summary>
        /// Retrieves the real-time weather forecast for a specific <see cref="Location"/> by its ID.
        /// Calls the external weather service using the location's latitude and longitude.
        /// </summary>
        /// <param name="id">The unique identifier of the location.</param>
        /// <returns>
        /// 200 OK with forecast data if successful;  
        /// 400 Bad Request if the ID is invalid;  
        /// 404 Not Found if the location does not exist;  
        /// 503 Service Unavailable if the weather service fails;  
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
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
