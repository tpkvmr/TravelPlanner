using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.DTOs.Response;
using TravelPlanner.Application.Services.Interfaces;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Interfaces;
using TravelPlanner.Core.Models;

namespace TravelPlanner.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IRepository<AnalyticsEvent> _eventRepository;
        private readonly IRepository<UserActivityLog> _activityLogRepository;
        private readonly IRepository<SearchHistory> _searchHistoryRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<PointOfInterest> _poiRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            IRepository<AnalyticsEvent> eventRepository,
            IRepository<UserActivityLog> activityLogRepository,
            IRepository<SearchHistory> searchHistoryRepository,
            IRepository<Trip> tripRepository,
            IRepository<PointOfInterest> poiRepository,
            IRepository<ApplicationUser> userRepository,
            ILogger<AnalyticsService> logger)
        {
            _eventRepository = eventRepository;
            _activityLogRepository = activityLogRepository;
            _searchHistoryRepository = searchHistoryRepository;
            _tripRepository = tripRepository;
            _poiRepository = poiRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AnalyticsResponseDto> GetAnalytics(AnalyticsRequestDto request)
        {
            var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = request.EndDate ?? DateTime.UtcNow;

            var analyticsData = await GetSystemAnalytics(startDate, endDate);

            var response = new AnalyticsResponseDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                GeneratedAt = DateTime.UtcNow,
                Summary = CreateSummary(analyticsData),
                Charts = await CreateCharts(startDate, endDate),
                Trends = await CalculateTrends(startDate, endDate),
                TopLists = await GetTopLists(startDate, endDate),
                Recommendations = await GenerateAnalyticsRecommendations(analyticsData)
            };

            if (request.IncludeRawData)
            {
                response.RawData = analyticsData;
            }

            return response;
        }

        public async Task LogEvent(string userId, string eventName, object eventData = null)
        {
            try
            {
                var analyticsEvent = new AnalyticsEvent
                {
                    EventName = eventName,
                    UserId = userId,
                    EventData = eventData != null ? System.Text.Json.JsonSerializer.Serialize(eventData) : null,
                    EventTime = DateTime.UtcNow,
                    Source = "API",
                    IsSuccess = true
                };

                await _eventRepository.AddAsync(analyticsEvent);

                _logger.LogDebug($"Logged event {eventName} for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging event {eventName} for user {userId}");
            }
        }

        public async Task LogUserActivity(string userId, string activityType, string description, object metadata = null)
        {
            try
            {
                var activity = new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = activityType,
                    Description = description,
                    Metadata = metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null,
                    CreatedAt = DateTime.UtcNow,
                    DeviceType = "Web"
                };

                await _activityLogRepository.AddAsync(activity);

                await LogEvent(userId, $"UserActivity_{activityType}", new
                {
                    Description = description,
                    Metadata = metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging user activity for user {userId}");
            }
        }

        public async Task LogSearch(string userId, string query, string searchType, object filters = null)
        {
            try
            {
                var searchHistory = new SearchHistory
                {
                    UserId = userId,
                    Query = query,
                    SearchType = searchType,
                    Filters = filters != null ? System.Text.Json.JsonSerializer.Serialize(filters) : null,
                    SearchedAt = DateTime.UtcNow,
                    IsSuccessful = true
                };

                await _searchHistoryRepository.AddAsync(searchHistory);

                await LogEvent(userId, "SearchPerformed", new
                {
                    Query = query,
                    SearchType = searchType,
                    Filters = filters
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging search for user {userId}");
            }
        }

        public async Task<AnalyticsData> GetSystemAnalytics(DateTime startDate, DateTime endDate)
        {
            var data = new AnalyticsData
            {
                PeriodStart = startDate,
                PeriodEnd = endDate
            };

            data.TotalUsers = await _userRepository.Query().CountAsync();
            data.NewUsers = await _userRepository.Query()
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .CountAsync();

            var activeUserIds = await _activityLogRepository.Query()
                .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .Select(a => a.UserId)
                .Distinct()
                .CountAsync();

            data.ActiveUsers = activeUserIds;

            var userSessions = await _activityLogRepository.Query()
                .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .GroupBy(a => a.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    FirstActivity = g.Min(a => a.CreatedAt),
                    LastActivity = g.Max(a => a.CreatedAt)
                })
                .ToListAsync();

            if (userSessions.Count > 0)
            {
                data.AverageSessionDuration = userSessions
                    .Select(s => (s.LastActivity - s.FirstActivity).TotalMinutes)
                    .Average();
            }

            data.TotalTrips = await _tripRepository.Query().CountAsync();
            data.TripsCreated = await _tripRepository.Query()
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .CountAsync();

            data.TripsCompleted = await _tripRepository.Query()
                .Where(t => t.EndDate >= startDate && t.EndDate <= endDate) 
                .CountAsync();

            var completedTrips = await _tripRepository.Query()
                .Where(t => t.StartDate.HasValue && t.EndDate.HasValue)
                .Select(t => new
                {
                    Duration = (t.EndDate.Value - t.StartDate.Value).TotalDays
                })
                .ToListAsync();

            if (completedTrips.Count > 0)
            {
                data.AverageTripDuration = completedTrips.Average(t => t.Duration);
            }

            data.TotalPOIs = await _poiRepository.Query().CountAsync();
            data.POIsViewed = await _activityLogRepository.Query()
                .Where(a => a.ActivityType == "ViewPOI" &&
                           a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .CountAsync();

            data.POIsBookmarked = await _activityLogRepository.Query()
                .Where(a => a.ActivityType == "BookmarkPOI" &&
                           a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .CountAsync();

            data.TotalSearches = await _searchHistoryRepository.Query()
                .Where(s => s.SearchedAt >= startDate && s.SearchedAt <= endDate)
                .CountAsync();

            var successfulSearches = await _searchHistoryRepository.Query()
                .Where(s => s.SearchedAt >= startDate && s.SearchedAt <= endDate && s.IsSuccessful)
                .CountAsync();

            data.SearchSuccessRate = data.TotalSearches > 0
                ? (double)successfulSearches / data.TotalSearches * 100
                : 0;

            data.TopSearchTerms = await _searchHistoryRepository.Query()
                .Where(s => s.SearchedAt >= startDate && s.SearchedAt <= endDate)
                .GroupBy(s => s.Query)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToListAsync();

            if (data.TopCategories == null)
                data.TopCategories = new Dictionary<string, int>();

            var categories = await _tripRepository.Query()
                .Where(t => t.Category != null)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            foreach (var category in categories)
            {
                data.TopCategories[category.Category] = category.Count;
            }

            var recommendationEvents = await _eventRepository.Query()
                .Where(e => e.EventName.Contains("Recommendation") &&
                           e.EventTime >= startDate && e.EventTime <= endDate)
                .ToListAsync();

            var generated = recommendationEvents.Count(e => e.EventName == "RecommendationGenerated");
            var accepted = recommendationEvents.Count(e => e.EventName == "RecommendationAccepted");

            data.RecommendationsGenerated = generated;
            data.RecommendationsViewed = recommendationEvents.Count(e => e.EventName == "RecommendationViewed");
            data.RecommendationsAccepted = accepted;
            data.AcceptanceRate = generated > 0 ? (double)accepted / generated * 100 : 0;

            return data;
        }

        public async Task<Dictionary<string, object>> GetUserAnalytics(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var analytics = new Dictionary<string, object>();

            var activities = await _activityLogRepository.Query()
                .Where(a => a.UserId == userId && a.CreatedAt >= start && a.CreatedAt <= end)
                .GroupBy(a => a.ActivityType)
                .Select(g => new { ActivityType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(a => a.ActivityType, a => a.Count);

            analytics["Activities"] = activities;

            var userTrips = await _tripRepository.Query()
                .Where(t => t.UserTrips.Any(ut => ut.UserId == userId))
                .CountAsync();

            analytics["TotalTrips"] = userTrips;

            var searches = await _searchHistoryRepository.Query()
                .Where(s => s.UserId == userId && s.SearchedAt >= start && s.SearchedAt <= end)
                .CountAsync();

            analytics["TotalSearches"] = searches;

            var recommendations = await _eventRepository.Query()
                .Where(e => e.UserId == userId &&
                           e.EventName.Contains("Recommendation") &&
                           e.EventTime >= start && e.EventTime <= end)
                .GroupBy(e => e.EventName)
                .Select(g => new { EventName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(r => r.EventName, r => r.Count);

            analytics["Recommendations"] = recommendations;

            return analytics;
        }

        public async Task<Dictionary<string, object>> GetTripAnalytics(int tripId)
        {
            var analytics = new Dictionary<string, object>();

            var views = await _activityLogRepository.Query()
                .Where(a => a.TripId == tripId && a.ActivityType == "ViewTrip")
                .CountAsync();

            analytics["TotalViews"] = views;

            var bookmarks = await _activityLogRepository.Query()
                .Where(a => a.TripId == tripId && a.ActivityType == "BookmarkTrip")
                .CountAsync();

            analytics["TotalBookmarks"] = bookmarks;

            var userInteractions = await _activityLogRepository.Query()
                .Where(a => a.TripId == tripId)
                .GroupBy(a => a.UserId)
                .CountAsync();

            analytics["UniqueUsers"] = userInteractions;

            var firstView = await _activityLogRepository.Query()
                .Where(a => a.TripId == tripId)
                .OrderBy(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            var lastView = await _activityLogRepository.Query()
                .Where(a => a.TripId == tripId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (firstView != null)
            {
                analytics["FirstView"] = firstView.CreatedAt;
                analytics["LastView"] = lastView?.CreatedAt;
            }

            return analytics;
        }

        public async Task<bool> GenerateReport(string userId, AnalyticsRequestDto request, string format = "pdf")
        {
            try
            {
                var analytics = await GetAnalytics(request);

                switch (format.ToLower())
                {
                    case "pdf":
                        await GeneratePdfReport(analytics, userId);
                        break;

                    case "excel":
                        await GenerateExcelReport(analytics, userId);
                        break;

                    case "csv":
                        await GenerateCsvReport(analytics, userId);
                        break;

                    case "json":
                        await GenerateJsonReport(analytics, userId);
                        break;
                }

                await LogEvent(userId, "ReportGenerated", new
                {
                    Format = format,
                    Period = new { request.StartDate, request.EndDate },
                    Request = request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating report for user {userId}");
                return false;
            }
        }

        public async Task<List<AnalyticsEvent>> GetRecentEvents(int limit = 100)
        {
            return await _eventRepository.Query()
                .OrderByDescending(e => e.EventTime)
                .Take(limit)
                .ToListAsync();
        }

        private Dictionary<string, object> CreateSummary(AnalyticsData data)
        {
            return new Dictionary<string, object>
            {
                { "TotalUsers", data.TotalUsers },
                { "NewUsers", data.NewUsers },
                { "ActiveUsers", data.ActiveUsers },
                { "AverageSessionDuration", $"{data.AverageSessionDuration:F1} min" },
                { "TotalTrips", data.TotalTrips },
                { "TripsCreated", data.TripsCreated },
                { "TripsCompleted", data.TripsCompleted },
                { "AverageTripDuration", $"{data.AverageTripDuration:F1} days" },
                { "TotalPOIs", data.TotalPOIs },
                { "POIsViewed", data.POIsViewed },
                { "POIsBookmarked", data.POIsBookmarked },
                { "TotalSearches", data.TotalSearches },
                { "SearchSuccessRate", $"{data.SearchSuccessRate:F1}%" },
                { "RecommendationAcceptanceRate", $"{data.AcceptanceRate:F1}%" },
                { "RecommendationsGenerated", data.RecommendationsGenerated },
                { "RecommendationsViewed", data.RecommendationsViewed },
                { "RecommendationsAccepted", data.RecommendationsAccepted }
            };
        }

        private async Task<Dictionary<string, ChartDataDto>> CreateCharts(DateTime startDate, DateTime endDate)
        {
            var charts = new Dictionary<string, ChartDataDto>();

            var registrations = await _userRepository.Query()
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            charts.Add("UserRegistrations", new ChartDataDto
            {
                Type = "Line",
                Labels = registrations.Select(r => r.Date.ToString("MMM dd")).ToList(),
                Datasets = new List<object>
                {
                    new { Label = "Registrations", Data = registrations.Select(r => r.Count).ToList() }
                }
            });

            var tripCreations = await _tripRepository.Query()
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            charts.Add("TripCreations", new ChartDataDto
            {
                Type = "Bar",
                Labels = tripCreations.Select(t => t.Date.ToString("MMM dd")).ToList(),
                Datasets = new List<object>
                {
                    new { Label = "Trips Created", Data = tripCreations.Select(t => t.Count).ToList() }
                }
            });

            var activities = await _activityLogRepository.Query()
                .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .GroupBy(a => a.ActivityType)
                .Select(g => new { ActivityType = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            charts.Add("TopActivities", new ChartDataDto
            {
                Type = "Pie",
                Labels = activities.Select(a => a.ActivityType).ToList(),
                Datasets = new List<object>
                {
                    new { Label = "Activities", Data = activities.Select(a => a.Count).ToList() }
                }
            });

            return charts;
        }

        private async Task<Dictionary<string, TrendDataDto>> CalculateTrends(DateTime startDate, DateTime endDate)
        {
            var trends = new Dictionary<string, TrendDataDto>();

            var previousStartDate = startDate.AddDays(-(endDate - startDate).Days);
            var previousEndDate = startDate;

            var currentUsers = await _userRepository.Query()
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .CountAsync();

            var previousUsers = await _userRepository.Query()
                .Where(u => u.CreatedAt >= previousStartDate && u.CreatedAt <= previousEndDate)
                .CountAsync();

            trends.Add("Users", new TrendDataDto
            {
                Dates = new List<DateTime> { startDate, endDate },
                Values = new List<decimal> { previousUsers, currentUsers },
                ChangePercentage = previousUsers > 0
                    ? ((decimal)currentUsers - previousUsers) / previousUsers * 100
                    : 100,
                TrendDirection = currentUsers > previousUsers ? "Up" : "Down"
            });

            var currentTrips = await _tripRepository.Query()
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .CountAsync();

            var previousTrips = await _tripRepository.Query()
                .Where(t => t.CreatedAt >= previousStartDate && t.CreatedAt <= previousEndDate)
                .CountAsync();

            trends.Add("Trips", new TrendDataDto
            {
                Dates = new List<DateTime> { startDate, endDate },
                Values = new List<decimal> { previousTrips, currentTrips },
                ChangePercentage = previousTrips > 0
                    ? ((decimal)currentTrips - previousTrips) / previousTrips * 100
                    : 100,
                TrendDirection = currentTrips > previousTrips ? "Up" : "Down"
            });

            return trends;
        }

        private async Task<Dictionary<string, List<TopItemDto>>> GetTopLists(DateTime startDate, DateTime endDate)
        {
            var topLists = new Dictionary<string, List<TopItemDto>>();

            var topTrips = await _activityLogRepository.Query()
                .Where(a => a.ActivityType == "ViewTrip" &&
                           a.CreatedAt >= startDate && a.CreatedAt <= endDate &&
                           a.TripId != null)
                .GroupBy(a => a.TripId)
                .Select(g => new
                {
                    TripId = g.Key,
                    Count = g.Count(),
                    Trip = _tripRepository.Query().FirstOrDefault(t => t.Id == g.Key)
                })
                .Where(x => x.Trip != null)
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => new TopItemDto
                {
                    Name = (string)x.Trip.Title, 
                    Value = x.Count,
                    Rank = 0,
                    Change = 0
                })
                .ToListAsync();

            for (int i = 0; i < topTrips.Count; i++)
            {
                topTrips[i].Rank = i + 1;
            }

            topLists.Add("TopTrips", topTrips);

            var topSearchTerms = await _searchHistoryRepository.Query()
                .Where(s => s.SearchedAt >= startDate && s.SearchedAt <= endDate)
                .GroupBy(s => s.Query)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new TopItemDto
                {
                    Name = g.Key,
                    Value = g.Count(),
                    Rank = 0,
                    Change = 0
                })
                .ToListAsync();

            for (int i = 0; i < topSearchTerms.Count; i++)
            {
                topSearchTerms[i].Rank = i + 1;
            }

            topLists.Add("TopSearchTerms", topSearchTerms);

            var topCategories = await _tripRepository.Query()
                .Where(t => t.Category != null)
                .GroupBy(t => t.Category)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new TopItemDto
                {
                    Name = g.Key,
                    Value = g.Count(),
                    Rank = 0,
                    Change = 0
                })
                .ToListAsync();

            for (int i = 0; i < topCategories.Count; i++)
            {
                topCategories[i].Rank = i + 1;
            }

            topLists.Add("TopCategories", topCategories);

            return topLists;
        }

        private async Task<List<AnalyticsRecommendationDto>> GenerateAnalyticsRecommendations(AnalyticsData data)
        {
            var recommendations = new List<AnalyticsRecommendationDto>();

            if (data.AcceptanceRate < 30)
            {
                recommendations.Add(new AnalyticsRecommendationDto
                {
                    Title = "Improve Recommendation Quality",
                    Description = $"Recommendation acceptance rate is low ({data.AcceptanceRate:F1}%). Consider refining the recommendation algorithm.",
                    Type = "Improvement",
                    Priority = "Medium",
                    Actions = new List<string>
                    {
                        "Review user feedback on recommendations",
                        "Adjust algorithm weights",
                        "Add more personalization factors"
                    }
                });
            }

            if (data.SearchSuccessRate < 60)
            {
                recommendations.Add(new AnalyticsRecommendationDto
                {
                    Title = "Enhance Search Functionality",
                    Description = $"Search success rate is low ({data.SearchSuccessRate:F1}%). Users may have difficulty finding what they need.",
                    Type = "Improvement",
                    Priority = "High",
                    Actions = new List<string>
                    {
                        "Improve search algorithm",
                        "Add autocomplete suggestions",
                        "Implement better filters"
                    }
                });
            }

            if (data.ActiveUsers < data.TotalUsers * 0.1)
            {
                recommendations.Add(new AnalyticsRecommendationDto
                {
                    Title = "Increase User Engagement",
                    Description = $"Only {data.ActiveUsers}/{data.TotalUsers} users are active. Consider sending re-engagement emails or notifications.",
                    Type = "Opportunity",
                    Priority = "High",
                    Actions = new List<string>
                    {
                        "Send personalized recommendations to inactive users",
                        "Create engagement campaigns",
                        "Add push notifications for new features"
                    }
                });
            }

            if (data.TripsCompleted < data.TripsCreated * 0.5)
            {
                recommendations.Add(new AnalyticsRecommendationDto
                {
                    Title = "Improve Trip Completion Rate",
                    Description = $"Only {data.TripsCompleted}/{data.TripsCreated} trips are completed. Users may need more guidance.",
                    Type = "Improvement",
                    Priority = "Medium",
                    Actions = new List<string>
                    {
                        "Add trip progress tracking",
                        "Send completion reminders",
                        "Provide trip planning assistance"
                    }
                });
            }

            return recommendations;
        }

        private async Task GeneratePdfReport(AnalyticsResponseDto analytics, string userId)
        {
            _logger.LogInformation($"Generating PDF report for user {userId}");
            await Task.CompletedTask;
        }

        private async Task GenerateExcelReport(AnalyticsResponseDto analytics, string userId)
        {
            _logger.LogInformation($"Generating Excel report for user {userId}");
            await Task.CompletedTask;
        }

        private async Task GenerateCsvReport(AnalyticsResponseDto analytics, string userId)
        {
            _logger.LogInformation($"Generating CSV report for user {userId}");
            await Task.CompletedTask;
        }

        private async Task GenerateJsonReport(AnalyticsResponseDto analytics, string userId)
        {
            _logger.LogInformation($"Generating JSON report for user {userId}");
            await Task.CompletedTask;
        }
    }
}

// initial commit