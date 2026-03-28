using System.Collections.Generic;
using System.Threading.Tasks;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.DTOs.Response;

namespace TravelPlanner.Application.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<RecommendationResponseDto>> GetPersonalizedRecommendations(GetRecommendationsDto request);
        Task<List<RecommendationResponseDto>> GetTrendingRecommendations(string userId, int limit = 10);
        Task<List<RecommendationResponseDto>> GetSimilarItems(int itemId, string itemType, string userId, int limit = 5);
        Task<List<RecommendationResponseDto>> GetContentBasedRecommendations(string userId, int limit = 10);
        Task<List<RecommendationResponseDto>> GetCollaborativeFilteringRecommendations(string userId, int limit = 10);
        Task<List<RecommendationResponseDto>> GetHybridRecommendations(GetRecommendationsDto request);
        Task UpdateUserPreferences(string userId, Dictionary<string, object> preferences);
        Task<List<string>> GetRecommendationReasons(int itemId, string itemType, string userId);
        Task<bool> LogRecommendationInteraction(string userId, int recommendationId, string interactionType);
    }
}