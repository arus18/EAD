using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

[Route("api/traindetails")]
[ApiController]
public class TrainDetailController : ControllerBase
{
    private readonly TrainDetailService _trainDetailService;

    public TrainDetailController(TrainDetailService trainDetailService)
    {
        _trainDetailService = trainDetailService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TrainDetail>>> Get()
    {
        var trainDetails = await _trainDetailService.GetAllAsync();
        return Ok(trainDetails);

    }
    [HttpGet("{id:length(24)}", Name = "GetTrainDetail")]
    public async Task<ActionResult<TrainDetail>> Get(string id)
    {
        var trainDetail = await _trainDetailService.GetByIdAsync(id);
        if (trainDetail == null)
        {
            return NotFound();
        }
        return Ok(trainDetail);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TrainDetail trainDetail)
    {
        string createdId = await _trainDetailService.CreateAsync(trainDetail);
        return CreatedAtRoute("GetTrainDetail", new { id = createdId }, trainDetail);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, TrainDetail trainDetail)
    {
        try
        {
            await _trainDetailService.UpdateAsync(id, trainDetail);
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
        await _trainDetailService.DeleteAsync(id);
        return NoContent();
    }
}