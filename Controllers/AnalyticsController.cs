using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.Services.Interfaces;

namespace TravelPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Analyst")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IAnalyticsService analyticsService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpPost("dashboard")]
        public async Task<ActionResult> GetDashboardAnalytics([FromBody] AnalyticsRequestDto request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var analytics = await _analyticsService.GetAnalytics(request);

                await _analyticsService.LogEvent(userId, "AnalyticsDashboardViewed", new
                {
                    PeriodStart = request.StartDate,
                    PeriodEnd = request.EndDate,
                    Metrics = request.Metrics
                });

                return Ok(new
                {
                    Success = true,
                    Data = analytics,
                    GeneratedAt = analytics.GeneratedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard analytics");
                return StatusCode(500, new { Success = false, Message = "Error getting analytics" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserAnalytics(string userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (currentUserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                var analytics = await _analyticsService.GetUserAnalytics(userId, startDate, endDate);

                return Ok(new
                {
                    Success = true,
                    Data = analytics,
                    UserId = userId,
                    Period = new { Start = startDate, End = endDate }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user analytics for {userId}");
                return StatusCode(500, new { Success = false, Message = "Error getting user analytics" });
            }
        }

        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult> GetTripAnalytics(int tripId)
        {
            try
            {
                var analytics = await _analyticsService.GetTripAnalytics(tripId);

                return Ok(new
                {
                    Success = true,
                    Data = analytics,
                    TripId = tripId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting trip analytics for {tripId}");
                return StatusCode(500, new { Success = false, Message = "Error getting trip analytics" });
            }
        }

        [HttpPost("events/log")]
        [AllowAnonymous]
        public async Task<ActionResult> LogEvent([FromBody] LogEventDto request)
        {
            try
            {
                await _analyticsService.LogEvent(request.UserId, request.EventName, request.EventData);

                return Ok(new { Success = true, Message = "Event logged successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging event {request.EventName}");
                return StatusCode(500, new { Success = false, Message = "Error logging event" });
            }
        }

        [HttpGet("recent-events")]
        public async Task<ActionResult> GetRecentEvents([FromQuery] int limit = 100)
        {
            try
            {
                var events = await _analyticsService.GetRecentEvents(limit);

                return Ok(new
                {
                    Success = true,
                    Data = events,
                    Count = events.Count,
                    RetrievedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent events");
                return StatusCode(500, new { Success = false, Message = "Error getting events" });
            }
        }

        [HttpPost("generate-report")]
        public async Task<ActionResult> GenerateReport([FromBody] AnalyticsRequestDto request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var success = await _analyticsService.GenerateReport(userId, request, request.Format);

                if (!success)
                    return BadRequest(new { Success = false, Message = "Failed to generate report" });

                return Ok(new
                {
                    Success = true,
                    Message = "Report generated successfully",
                    Format = request.Format,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return StatusCode(500, new { Success = false, Message = "Error generating report" });
            }
        }

        public class LogEventDto
        {
            public string UserId { get; set; }
            public string EventName { get; set; }
            public object EventData { get; set; }
        }
    }
}