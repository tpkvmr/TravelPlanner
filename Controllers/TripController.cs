using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TravelPlanner.Application.DTOs;
using TravelPlanner.Application.DTOs.Patch;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Interfaces.Services;
using TravelPlanner.Core.Models.Requests;
using TravelPlanner.Core.Models.Results;
using TravelPlanner.Infrastructure.Data;

namespace TravelPlanner.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TripController : ControllerBase
    {
        private readonly ITripService _service;
        private readonly ApplicationDbContext _context;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        private string UserEmail => User.FindFirstValue(ClaimTypes.Email) ??
                                    User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "";

        public TripController(ITripService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        #region POST

        [HttpPost("create")]
        [ProducesResponseType(typeof(TripIdResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TripIdResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripDto dto)
        {
            var request = new CreateTripRequest(dto.Title, dto.Description, dto.StartDate, dto.EndDate, dto.Latitude, dto.Longitude, UserEmail);
            var result = await _service.CreateAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(HandleError(result.Errors!));
        }

        [HttpPost("join")]
        [ProducesResponseType(typeof(TripIdResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TripIdResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinTrip([FromBody] JoinTripDto dto)
        {
            var request = new JoinTripRequest(dto.TripId, dto.Role, UserEmail);
            var result = await _service.JoinAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(HandleError(result.Errors!));
        }

        #endregion

        #region GET

        [HttpGet("getAll")]
        [ProducesResponseType(typeof(List<Trip>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Trip>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllTrips()
        {
            var result = await _service.GetAllAsync(UserId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(HandleError(result.Errors!));
        }

        [HttpGet("getById/{id}")]
        [ProducesResponseType(typeof(TripResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TripResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTrip(int id)
        {
            var result = await _service.GetByIdAsync(new(id, UserId));

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(HandleError(result.Errors!));
        }

        #endregion

        #region PATCH

        [HttpPatch("update/{id}/title")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTripTitle(int id, [FromBody] string newTitle)
        {
            var request = new UpdateTextRequest(id, newTitle, UserId);
            var result = await _service.UpdateTitleAsync(request);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(HandleError(result.Errors!));
        }

        [HttpPatch("update/{id}/description")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTripDescription(int id, [FromBody] string newDescription)
        {
            var request = new UpdateTextRequest(id, newDescription, UserId);
            var result = await _service.UpdateDescriptionAsync(request);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(HandleError(result.Errors!));
        }

        [HttpPatch("update/{id}/dates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTripDates(int id, [FromBody] UpdateDatesDto datesDto)
        {
            var request = new UpdateDatesRequest(id, datesDto.StartDate, datesDto.EndDate, UserId);
            var result = await _service.UpdateDatesAsync(request);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(HandleError(result.Errors!));
        }

        #endregion

        #region DELETE

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var request = new UserResourceIdRequest(id, UserId);
            var result = await _service.DeleteAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(HandleError(result.Errors!));
        }

        [HttpPost("leave/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LeaveTrip(int id)
        {
            var request = new UserResourceIdRequest(id, UserId);
            var result = await _service.LeaveAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        #endregion

        [HttpPost("{tripId}/budget")]
        public async Task<IActionResult> SetTripBudget(int tripId, [FromBody] TripBudgetDto dto)
        {
            var trip = await _context.Trips
                .Include(t => t.DailyBudgets)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                return NotFound("Trip not found");

            trip.TotalBudget = dto.TotalBudget;

            _context.DailyBudgets.RemoveRange(trip.DailyBudgets);

            foreach (var day in dto.DailyBudgets)
            {
                trip.DailyBudgets.Add(new DailyBudget
                {
                    Date = day.Date,
                    PlannedAmount = day.PlannedAmount
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        private IActionResult HandleError(IEnumerable<string> errors)
        {
            return errors?.FirstOrDefault() switch
            {
                "Forbidden" => Forbid(),
                "User not found" => Unauthorized(errors),
                "Trip not found" => NotFound(errors),
                "User already in trip" => Conflict(errors),
                _ => BadRequest(errors)
            };
        }
    }
}