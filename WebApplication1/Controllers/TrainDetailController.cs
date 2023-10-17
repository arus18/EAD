using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

[Route("api/traindetails")] // Define the base route for this controller.
[ApiController] // Declare that this class is an API controller.
public class TrainDetailController : ControllerBase
{
    private readonly TrainDetailService _trainDetailService; // Dependency injection of TrainDetailService.

    public TrainDetailController(TrainDetailService trainDetailService)
    {
        _trainDetailService = trainDetailService; // Initialize the TrainDetailService.
    }

    [HttpGet] // Handle HTTP GET requests.
    public async Task<ActionResult<List<TrainDetail>>> Get()
    {
        // Retrieve a list of train details asynchronously.
        var trainDetails = await _trainDetailService.GetAllAsync();
        return Ok(trainDetails); // Return the list of train details as an HTTP response.
    }

    [HttpGet("{id:length(24)}", Name = "GetTrainDetail")] // Handle HTTP GET requests with a specific ID.
    public async Task<ActionResult<TrainDetail>> Get(string id)
    {
        // Retrieve a specific train detail by its ID asynchronously.
        var trainDetail = await _trainDetailService.GetByIdAsync(id);
        if (trainDetail == null)
        {
            return NotFound(); // If the train detail is not found, return a 404 Not Found response.
        }
        return Ok(trainDetail); // Return the train detail as an HTTP response.
    }

    [HttpPost] // Handle HTTP POST requests.
    public async Task<IActionResult> Create(TrainDetail trainDetail)
    {
        // Create a new train detail by calling the service method.
        string createdId = await _trainDetailService.CreateAsync(trainDetail);
        return CreatedAtRoute("GetTrainDetail", new { id = createdId }, trainDetail);
        // Return a 201 Created response with a location header pointing to the newly created resource.
    }

    [HttpPut("{id:length(24)}")] // Handle HTTP PUT requests for a specific ID.
    public async Task<IActionResult> Update(string id, TrainDetail trainDetail)
    {
        try
        {
            // Update an existing train detail by its ID.
            await _trainDetailService.UpdateAsync(id, trainDetail);
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
        // Delete a train detail by its ID.
        await _trainDetailService.DeleteAsync(id);
        return NoContent(); // Return a 204 No Content response indicating success.
    }
}
