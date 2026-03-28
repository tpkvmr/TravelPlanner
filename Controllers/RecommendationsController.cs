using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.DTOs.Response;
using TravelPlanner.Application.Services.Interfaces;

namespace TravelPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<RecommendationsController> _logger;

        public RecommendationsController(
            IRecommendationService recommendationService,
            IAnalyticsService analyticsService,
            ILogger<RecommendationsController> logger)
        {
            _recommendationService = recommendationService;
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpPost("personalized")]
        public async Task<ActionResult<List<RecommendationResponseDto>>> GetPersonalizedRecommendations(
            [FromBody] GetRecommendationsDto request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                request.UserId = userId;

                var recommendations = await _recommendationService.GetPersonalizedRecommendations(request);

                return Ok(new
                {
                    Success = true,
                    Data = recommendations,
                    Count = recommendations.Count,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized recommendations");
                return StatusCode(500, new { Success = false, Message = "Error getting recommendations" });
            }
        }

        [HttpGet("trending")]
        public async Task<ActionResult<List<RecommendationResponseDto>>> GetTrendingRecommendations(
            [FromQuery] int limit = 10)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var recommendations = await _recommendationService.GetTrendingRecommendations(userId, limit);

                return Ok(new
                {
                    Success = true,
                    Data = recommendations,
                    Type = "Trending",
                    Count = recommendations.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending recommendations");
                return StatusCode(500, new { Success = false, Message = "Error getting trending recommendations" });
            }
        }

        [HttpGet("similar/{itemType}/{itemId}")]
        public async Task<ActionResult<List<RecommendationResponseDto>>> GetSimilarItems(
            string itemType, int itemId, [FromQuery] int limit = 5)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var recommendations = await _recommendationService.GetSimilarItems(itemId, itemType, userId, limit);

                return Ok(new
                {
                    Success = true,
                    Data = recommendations,
                    SourceItemId = itemId,
                    SourceItemType = itemType,
                    Count = recommendations.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting similar items for {itemType} {itemId}");
                return StatusCode(500, new { Success = false, Message = "Error getting similar items" });
            }
        }

        [HttpPost("interaction/{recommendationId}")]
        public async Task<ActionResult> LogInteraction(
            int recommendationId, [FromBody] InteractionDto interaction)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var success = await _recommendationService.LogRecommendationInteraction(
                    userId, recommendationId, interaction.InteractionType);

                if (!success)
                    return NotFound(new { Success = false, Message = "Recommendation not found" });

                return Ok(new { Success = true, Message = "Interaction logged" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging interaction for recommendation {recommendationId}");
                return StatusCode(500, new { Success = false, Message = "Error logging interaction" });
            }
        }

        [HttpGet("user-preferences")]
        public async Task<ActionResult<Dictionary<string, object>>> GetUserPreferences()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                return Ok(new
                {
                    Success = true,
                    Data = new Dictionary<string, object>
                    {
                        { "PreferredCategories", new List<string> { "Adventure", "Cultural" } },
                        { "BudgetRange", "Medium" },
                        { "TravelSeason", "Summer" }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user preferences");
                return StatusCode(500, new { Success = false, Message = "Error getting preferences" });
            }
        }

        public class InteractionDto
        {
            public string InteractionType { get; set; } 
            public string Feedback { get; set; }
        }
    }
}