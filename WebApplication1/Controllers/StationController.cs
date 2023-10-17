using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

[Route("api/stations")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly StationService _stationService;

    public StationController(StationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Station>>> Get()
    {
        var stations = await _stationService.GetAllAsync();
        return Ok(stations);
    }

    [HttpGet("{id:length(24)}", Name = "GetStation")]
    public async Task<ActionResult<Station>> Get(string id)
    {
        var station = await _stationService.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }
        return Ok(station);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Station station)
    {
        string createdId = await _stationService.CreateAsync(station);
        return CreatedAtRoute("GetStation", new { id = createdId }, station);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Station station)
    {
        try
        {
            await _stationService.UpdateAsync(id, station);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _stationService.DeleteAsync(id);
        return NoContent();
    }
}