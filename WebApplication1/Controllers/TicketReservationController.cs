//Ticket Reservation Controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketReservationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TicketBookingService _reservationService;
        private readonly ILogger<TicketReservationController> _logger;

        public TicketReservationController(IConfiguration configuration, TicketBookingService reservationService,
            ILogger<TicketReservationController> logger)
        {
            _configuration = configuration;
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<TicketReservation> reservations = await _reservationService.GetAsync();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching ticket reservations.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                TicketReservation reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound();
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching ticket reservation with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TicketReservation reservation)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(token);
    
            // Extract the user's ID from the claim.
            string userId = tokenS.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.NameId)?.Value;
    
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID claim not found.");
            }

            reservation.UserId = userId; // Set the UserId in the reservation object.

            try
            {
                // Attempt to create the reservation using the service.
                string reservationId = await _reservationService.CreateAsync(reservation);

                // If creation was successful, return a 201 Created response.
                return CreatedAtAction(nameof(GetById), new { id = reservationId }, reservation);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors returned by the service.
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a ticket reservation.");
                return StatusCode(500, "Internal server error.");
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TicketReservation reservation)
        {
            try
            {
                // Attempt to update the reservation using the service.
                await _reservationService.UpdateAsync(id, reservation);

                // If the update was successful, return a 200 OK response.
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors returned by the service.
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating ticket reservation with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Attempt to delete the reservation using the service.
                await _reservationService.DeleteAsync(id);

                // If the deletion was successful, return a 200 OK response.
                return Ok("Ticket reservation deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting ticket reservation with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReservationsByUserId(string userId)
        {
            try
            {
                List<TicketReservation> userReservations =
                    await _reservationService.GetReservationsByUserIdAsync(userId);
                return Ok(userReservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching reservations for user with ID: {userId}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}