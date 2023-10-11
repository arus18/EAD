//Train Schedule Controller
// This controller manages train schedules.

using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
namespace WebApplication1.Controllers;

[Route("api/train-schedules")]
[ApiController]
public class TrainScheduleController : ControllerBase
{
    private readonly TrainScheduleService _scheduleService;

    // Constructor for TrainScheduleController
    public TrainScheduleController(TrainScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // Get all train schedules
    [HttpGet]
    public async Task<ActionResult<List<TrainSchedule>>> GetAllTrainSchedules()
    {
        var schedules = await _scheduleService.GetAllAsync();
        return Ok(schedules);
    }

    // Get a train schedule by ID
    [HttpGet("{id:length(24)}", Name = "GetTrainSchedule")]
    public async Task<ActionResult<TrainSchedule>> GetTrainScheduleById(string id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }
        return Ok(schedule);
    }

    // Create a new train schedule
    [HttpPost]
    public async Task<ActionResult<string>> CreateTrainSchedule(TrainSchedule schedule)
    {
        try
        {
            var id = await _scheduleService.CreateAsync(schedule);
            return CreatedAtRoute("GetTrainSchedule", new { id }, id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Update an existing train schedule by ID
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateTrainSchedule(string id, TrainSchedule schedule)
    {
        try
        {
            await _scheduleService.UpdateAsync(id, schedule);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // Update an existing train schedule by ID
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteTrainSchedule(string id)
    {
        await _scheduleService.DeleteAsync(id);
        return NoContent();
    }
}