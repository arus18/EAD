using Microsoft.AspNetCore.Mvc; // Import the required MVC namespace.
using System; // Import the System namespace for fundamental types.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for lists.
using System.Threading.Tasks; // Import the System.Threading.Tasks namespace for asynchronous operations.
using WebApplication1.Models; // Import the models required for this controller.
using WebApplication1.Services; // Import the services required for this controller.

[Route("api/stations")] // Define the base route for this controller.
[ApiController] // Declare that this class is an API controller.
public class StationController : ControllerBase
{
    private readonly StationService _stationService; // Dependency injection of StationService.

    public StationController(StationService stationService)
    {
        _stationService = stationService; // Initialize the StationService.
    }

    [HttpGet] // Handle HTTP GET requests.
    public async Task<ActionResult<List<Station>>> Get()
    {
        // Retrieve a list of stations asynchronously.
        var stations = await _stationService.GetAllAsync();
        return Ok(stations); // Return the list of stations as an HTTP response.
    }

    [HttpGet("{id:length(24)}", Name = "GetStation")] // Handle HTTP GET requests with a specific ID.
    public async Task<ActionResult<Station>> Get(string id)
    {
        // Retrieve a specific station by its ID asynchronously.
        var station = await _stationService.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound(); // If the station is not found, return a 404 Not Found response.
        }
        return Ok(station); // Return the station as an HTTP response.
    }

    [HttpPost] // Handle HTTP POST requests.
    public async Task<IActionResult> Create(Station station)
    {
        // Create a new station by calling the service method.
        string createdId = await _stationService.CreateAsync(station);
        return CreatedAtRoute("GetStation", new { id = createdId }, station);
        // Return a 201 Created response with a location header pointing to the newly created resource.
    }

    [HttpPut("{id:length(24)}")] // Handle HTTP PUT requests for a specific ID.
    public async Task<IActionResult> Update(string id, Station station)
    {
        try
        {
            // Update an existing station by its ID.
            await _stationService.UpdateAsync(id, station);
            return NoContent(); // Return a 204 No Content response indicating success.
        }
        catch (ArgumentException)
        {
            return NotFound(); // If the update operation fails due to an invalid argument, return a 404 Not Found response.
        }
    }

    [HttpDelete("{id:length(24)}")] // Handle HTTP DELETE requests for a specific ID.
    public async Task<IActionResult> Delete(string id)
    {
        // Delete a station by its ID.
        await _stationService.DeleteAsync(id);
        return NoContent(); // Return a 204 No Content response indicating success.
    }
}
