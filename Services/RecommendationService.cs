using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.DTOs.Response;
using TravelPlanner.Application.Services.Interfaces;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Interfaces;
using TravelPlanner.Core.Models;

namespace TravelPlanner.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<PointOfInterest> _poiRepository;
        private readonly IRepository<Recommendation> _recommendationRepository;
        private readonly IRepository<UserPreference> _preferenceRepository;
        private readonly IRepository<UserActivityLog> _activityLogRepository;
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<RecommendationService> _logger;
        private readonly RecommendationSettings _settings;

        public RecommendationService(
            IRepository<Trip> tripRepository,
            IRepository<PointOfInterest> poiRepository,
            IRepository<Recommendation> recommendationRepository,
            IRepository<UserPreference> preferenceRepository,
            IRepository<UserActivityLog> activityLogRepository,
            IAnalyticsService analyticsService,
            ILogger<RecommendationService> logger,
            IOptions<RecommendationSettings> settings)
        {
            _tripRepository = tripRepository;
            _poiRepository = poiRepository;
            _recommendationRepository = recommendationRepository;
            _preferenceRepository = preferenceRepository;
            _activityLogRepository = activityLogRepository;
            _analyticsService = analyticsService;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<List<RecommendationResponseDto>> GetPersonalizedRecommendations(GetRecommendationsDto request)
        {
            try
            {
                _logger.LogInformation($"Getting personalized recommendations for user {request.UserId}");

                var recommendations = new List<RecommendationResponseDto>();

                var preferences = await _preferenceRepository.Query()
                    .Where(p => p.UserId == request.UserId)
                    .ToListAsync();

                var activities = await _activityLogRepository.Query()
                    .Where(a => a.UserId == request.UserId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(100)
                    .ToListAsync();

                var contentBased = await GetContentBasedRecommendations(request.UserId, request.Limit ?? 10);
                recommendations.AddRange(contentBased);

                if (request.RecommendationTypes == null ||
                    request.RecommendationTypes.Contains("Collaborative") ||
                    request.RecommendationTypes.Count == 0)
                {
                    var collaborative = await GetCollaborativeFilteringRecommendations(request.UserId, 5);
                    recommendations.AddRange(collaborative);
                }

                var trending = await GetTrendingRecommendations(request.UserId, 5);
                recommendations.AddRange(trending);

                if (!string.IsNullOrEmpty(request.Location) ||
                    (request.Latitude.HasValue && request.Longitude.HasValue))
                {
                    var locationBased = await GetLocationBasedRecommendations(request);
                    recommendations.AddRange(locationBased);
                }

                recommendations = recommendations
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .OrderByDescending(r => r.ConfidenceScore)
                    .Take(request.Limit ?? 10)
                    .ToList();

                await _analyticsService.LogEvent(
                    request.UserId,
                    "RecommendationGenerated",
                    new { Count = recommendations.Count, Types = recommendations.Select(r => r.Type).Distinct() }
                );

                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting recommendations for user {request.UserId}");
                throw;
            }
        }

        public async Task<List<RecommendationResponseDto>> GetContentBasedRecommendations(string userId, int limit = 10)
        {
            var userActivities = await _activityLogRepository.Query()
                .Where(a => a.UserId == userId &&
                           (a.ActivityType == "View" || a.ActivityType == "Bookmark"))
                .ToListAsync();

            var recommendations = new List<RecommendationResponseDto>();

            foreach (var activity in userActivities)
            {
                if (activity.TripId.HasValue)
                {
                    var similarTrips = await FindSimilarTrips(activity.TripId.Value, userId);
                    recommendations.AddRange(similarTrips);
                }
                else if (activity.PointOfInterestId.HasValue)
                {
                    var similarPOIs = await FindSimilarPOIs(activity.PointOfInterestId.Value, userId);
                    recommendations.AddRange(similarPOIs);
                }
            }

            return recommendations
                .OrderByDescending(r => r.ConfidenceScore)
                .Take(limit)
                .ToList();
        }

        public async Task<List<RecommendationResponseDto>> GetCollaborativeFilteringRecommendations(string userId, int limit = 10)
        {
            try
            {
                var currentUserPreferences = await _preferenceRepository.Query()
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                if (!currentUserPreferences.Any())
                    return new List<RecommendationResponseDto>();

                var similarUsers = await _preferenceRepository.Query()
                    .Where(p => p.UserId != userId)
                    .GroupBy(p => p.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        SimilarityScore = CalculateUserSimilarity(currentUserPreferences, g.ToList())
                    })
                    .Where(u => u.SimilarityScore > 0.3m)
                    .OrderByDescending(u => u.SimilarityScore)
                    .Take(5)
                    .ToListAsync();

                var recommendations = new List<RecommendationResponseDto>();

                foreach (var similarUser in similarUsers)
                {
                    var userActivities = await _activityLogRepository.Query()
                        .Where(a => a.UserId == similarUser.UserId &&
                                   (a.ActivityType == "View" || a.ActivityType == "Bookmark") &&
                                   a.TripId.HasValue)
                        .Select(a => a.TripId.Value)
                        .Distinct()
                        .Take(3)
                        .ToListAsync();

                    foreach (var tripId in userActivities)
                    {
                        var trip = await _tripRepository.GetByIdAsync(tripId);
                        if (trip != null)
                        {
                            recommendations.Add(new RecommendationResponseDto
                            {
                                Id = $"Trip_{trip.Id}",
                                Type = "Trip",
                                Title = trip.Title, 
                                Description = trip.Description,
                                ConfidenceScore = similarUser.SimilarityScore,
                                Reasons = new List<string>
                                {
                                    "Users with similar preferences liked this",
                                    $"Similarity score: {similarUser.SimilarityScore:P0}"
                                },
                                Location = trip.Location,
                                StartDate = trip.StartDate,
                                EndDate = trip.EndDate,
                                EstimatedBudget = trip.Budget
                            });
                        }
                    }
                }

                return recommendations
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .OrderByDescending(r => r.ConfidenceScore)
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting collaborative recommendations for user {userId}");
                return new List<RecommendationResponseDto>();
            }
        }

        private decimal CalculateUserSimilarity(List<UserPreference> user1Prefs, List<UserPreference> user2Prefs)
        {
            if (!user1Prefs.Any() || !user2Prefs.Any())
                return 0;

            decimal similarity = 0;
            int matchCount = 0;

            foreach (var pref1 in user1Prefs)
            {
                var matchingPref = user2Prefs.FirstOrDefault(p =>
                    p.PreferenceType == pref1.PreferenceType &&
                    p.PreferenceKey == pref1.PreferenceKey);

                if (matchingPref != null)
                {
                    decimal valueSimilarity = 1.0m;
                    if (pref1.PreferenceValue != matchingPref.PreferenceValue)
                    {
                        valueSimilarity = 0.5m; 
                    }

                    similarity += valueSimilarity * (pref1.Weight + matchingPref.Weight) / 2;
                    matchCount++;
                }
            }

            return matchCount > 0 ? similarity / matchCount : 0;
        }

        public async Task<List<RecommendationResponseDto>> GetTrendingRecommendations(string userId, int limit = 10)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-7);

                var trendingTrips = await _tripRepository.Query()
                    .Where(t => t.CreatedAt >= cutoffDate)
                    .OrderByDescending(t => t.ViewCount + t.BookmarkCount * 2) 
                    .Take(limit * 2) 
                    .ToListAsync();

                var userViewedTrips = await _activityLogRepository.Query()
                    .Where(a => a.UserId == userId &&
                               a.ActivityType == "View" &&
                               a.TripId.HasValue)
                    .Select(a => a.TripId.Value)
                    .ToListAsync();

                var recommendations = trendingTrips
                    .Where(t => !userViewedTrips.Contains(t.Id))
                    .Take(limit)
                    .Select(t => new RecommendationResponseDto
                    {
                        Id = $"Trip_{t.Id}",
                        Type = "Trip",
                        Title = t.Title, 
                        Description = t.Description,
                        ConfidenceScore = CalculateTrendingScore(t),
                        Reasons = new List<string>
                        {
                            "Trending now",
                            $"{t.ViewCount} views, {t.BookmarkCount} bookmarks"
                        },
                        Location = t.Location,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        EstimatedBudget = t.Budget,
                        ImageUrl = t.ImageUrl
                    })
                    .ToList();

                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting trending recommendations for user {userId}");
                return new List<RecommendationResponseDto>();
            }
        }

        private decimal CalculateTrendingScore(Trip trip)
        {
            var viewScore = Math.Min(trip.ViewCount / 100.0m, 1.0m);
            var bookmarkScore = Math.Min(trip.BookmarkCount / 50.0m, 1.0m);
            var ratingScore = (decimal)trip.AverageRating / 5.0m;

            
            return (viewScore * 0.4m) + (bookmarkScore * 0.4m) + (ratingScore * 0.2m);
        }

        public async Task<List<RecommendationResponseDto>> GetLocationBasedRecommendations(GetRecommendationsDto request)
        {
            try
            {
                var query = _tripRepository.Query();

                if (!string.IsNullOrEmpty(request.Location))
                {
                    query = query.Where(t => t.Location != null && t.Location.Contains(request.Location));
                }
                else if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                   
                    query = query.Where(t =>
                        t.Latitude >= request.Latitude - 1.0 &&
                        t.Latitude <= request.Latitude + 1.0 &&
                        t.Longitude >= request.Longitude - 1.0 &&
                        t.Longitude <= request.Longitude + 1.0);
                }

                var trips = await query
                    .OrderByDescending(t => t.ViewCount)
                    .Take(request.Limit ?? 10)
                    .ToListAsync();

                return trips.Select(t => new RecommendationResponseDto
                {
                    Id = $"Trip_{t.Id}",
                    Type = "Trip",
                    Title = t.Title, 
                    Description = t.Description,
                    ConfidenceScore = 0.7m, 
                    Reasons = new List<string>
                    {
                        $"Popular in {t.Location}",
                        "Based on your location preferences"
                    },
                    Location = t.Location,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    EstimatedBudget = t.Budget,
                    ImageUrl = t.ImageUrl
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location-based recommendations");
                return new List<RecommendationResponseDto>();
            }
        }

        private async Task<List<RecommendationResponseDto>> FindSimilarTrips(int tripId, string userId)
        {
            var sourceTrip = await _tripRepository.GetByIdAsync(tripId);
            if (sourceTrip == null) return new List<RecommendationResponseDto>();

            var similarTrips = await _tripRepository.Query()
                .Where(t => t.Id != tripId &&
                           (t.Location != null && t.Location.Contains(sourceTrip.Location ?? "")) ||
                           (t.Category == sourceTrip.Category) ||
                           (t.Activities != null && sourceTrip.Activities != null &&
                            t.Activities.Any(a => sourceTrip.Activities.Contains(a))))
                .Take(5)
                .ToListAsync();

            return similarTrips.Select(t => new RecommendationResponseDto
            {
                Id = $"Trip_{t.Id}",
                Type = "Trip",
                Title = t.Title, 
                Description = t.Description,
                ConfidenceScore = CalculateSimilarityScore(sourceTrip, t),
                Reasons = new List<string> { "Similar to trips you've viewed", $"Based on your interest in {sourceTrip.Location}" },
                Location = t.Location,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                EstimatedBudget = t.Budget,
                ImageUrl = t.ImageUrl
            }).ToList();
        }

        private async Task<List<RecommendationResponseDto>> FindSimilarPOIs(int poiId, string userId)
        {
            return new List<RecommendationResponseDto>();
        }

        private decimal CalculateSimilarityScore(Trip trip1, Trip trip2)
        {
            decimal score = 0;
            int factorCount = 0;

            if (trip1.Location == trip2.Location) { score += 0.3m; factorCount++; }
            if (trip1.Category == trip2.Category) { score += 0.2m; factorCount++; }

            if (trip1.Activities != null && trip2.Activities != null)
            {
                var commonActivities = trip1.Activities.Intersect(trip2.Activities).Count();
                var maxActivities = Math.Max(trip1.Activities.Count, trip2.Activities.Count);
                if (maxActivities > 0)
                {
                    score += (decimal)commonActivities / maxActivities * 0.3m;
                    if (commonActivities > 0) factorCount++;
                }
            }

            if (trip1.Budget.HasValue && trip2.Budget.HasValue && trip1.Budget.Value > 0 && trip2.Budget.Value > 0)
            {
                var budgetDiff = Math.Abs(trip1.Budget.Value - trip2.Budget.Value) / Math.Max(trip1.Budget.Value, trip2.Budget.Value);
                score += (1 - (decimal)budgetDiff) * 0.2m;
                factorCount++;
            }

            return factorCount > 0 ? score / factorCount : 0;
        }

        public async Task<bool> LogRecommendationInteraction(string userId, int recommendationId, string interactionType)
        {
            var recommendation = await _recommendationRepository.GetByIdAsync(recommendationId);
            if (recommendation == null) return false;

            if (interactionType == "view")
            {
                recommendation.IsViewed = true;
                recommendation.ViewedAt = DateTime.UtcNow;
            }
            else if (interactionType == "accept")
            {
                recommendation.IsAccepted = true;
                recommendation.AcceptedAt = DateTime.UtcNow;
            }

            await _recommendationRepository.UpdateAsync(recommendation);

            await _analyticsService.LogEvent(
                userId,
                $"Recommendation{interactionType}",
                new { RecommendationId = recommendationId, Type = recommendation.RecommendationType }
            );

            return true;
        }

        public async Task<List<RecommendationResponseDto>> GetSimilarItems(int itemId, string itemType, string userId, int limit = 5)
        {
            if (itemType == "Trip")
            {
                return await FindSimilarTrips(itemId, userId);
            }
            return new List<RecommendationResponseDto>();
        }

        public async Task UpdateUserPreferences(string userId, Dictionary<string, object> preferences)
        {
            try
            {
                var existingPreferences = await _preferenceRepository.Query()
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                foreach (var pref in preferences)
                {
                    var existingPref = existingPreferences.FirstOrDefault(p =>
                        p.PreferenceType == "UserDefined" &&
                        p.PreferenceKey == pref.Key);

                    if (existingPref != null)
                    {
                        existingPref.PreferenceValue = pref.Value.ToString();
                        existingPref.LastUpdated = DateTime.UtcNow;
                        existingPref.UsageCount++;
                        await _preferenceRepository.UpdateAsync(existingPref);
                    }
                    else
                    {
                        var newPreference = new UserPreference
                        {
                            UserId = userId,
                            PreferenceType = "UserDefined",
                            PreferenceKey = pref.Key,
                            PreferenceValue = pref.Value.ToString(),
                            LastUpdated = DateTime.UtcNow,
                            UsageCount = 1
                        };
                        await _preferenceRepository.AddAsync(newPreference);
                    }
                }

                _logger.LogInformation($"Updated preferences for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating preferences for user {userId}");
                throw;
            }
        }

        public async Task<List<string>> GetRecommendationReasons(int itemId, string itemType, string userId)
        {
            var reasons = new List<string>();

            if (itemType == "Trip")
            {
                var trip = await _tripRepository.GetByIdAsync(itemId);
                if (trip != null)
                {
                    reasons.Add($"Popular in {trip.Location}");
                    reasons.Add($"Category: {trip.Category}");

                    if (trip.AverageRating >= 4.0)
                        reasons.Add($"High rating: {trip.AverageRating}/5");

                    if (trip.ViewCount > 100)
                        reasons.Add($"Popular: {trip.ViewCount} views");
                }
            }

            return reasons;
        }

        public async Task<List<RecommendationResponseDto>> GetHybridRecommendations(GetRecommendationsDto request)
        {
            var allRecommendations = new List<RecommendationResponseDto>();

            var contentBased = await GetContentBasedRecommendations(request.UserId, 5);
            var collaborative = await GetCollaborativeFilteringRecommendations(request.UserId, 5);
            var trending = await GetTrendingRecommendations(request.UserId, 5);

            allRecommendations.AddRange(contentBased);
            allRecommendations.AddRange(collaborative);
            allRecommendations.AddRange(trending);

            foreach (var rec in allRecommendations)
            {
                if (rec.Reasons.Any(r => r.Contains("Similar to trips you've viewed")))
                    rec.ConfidenceScore *= _settings.AlgorithmWeights.GetValueOrDefault("ContentBased", 0.4m);
                else if (rec.Reasons.Any(r => r.Contains("Users with similar preferences")))
                    rec.ConfidenceScore *= _settings.AlgorithmWeights.GetValueOrDefault("Collaborative", 0.3m);
                else if (rec.Reasons.Any(r => r.Contains("Trending now")))
                    rec.ConfidenceScore *= _settings.AlgorithmWeights.GetValueOrDefault("Trending", 0.2m);
                else if (rec.Reasons.Any(r => r.Contains("Based on your location")))
                    rec.ConfidenceScore *= _settings.AlgorithmWeights.GetValueOrDefault("LocationBased", 0.1m);
            }

            return allRecommendations
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderByDescending(r => r.ConfidenceScore)
                .Take(request.Limit ?? 10)
                .ToList();
        }
    }

    public class RecommendationSettings
    {
        public string DefaultAlgorithm { get; set; } = "Hybrid";
        public int DefaultLimit { get; set; } = 10;
        public decimal SimilarityThreshold { get; set; } = 0.6m;
        public int MaxHistoryItems { get; set; } = 100;
        public Dictionary<string, decimal> AlgorithmWeights { get; set; } = new Dictionary<string, decimal>
        {
            { "ContentBased", 0.4m },
            { "Collaborative", 0.3m },
            { "Trending", 0.2m },
            { "LocationBased", 0.1m }
        };
    }
}