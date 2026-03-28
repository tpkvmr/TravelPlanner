using System.Threading.Tasks;
using TravelPlanner.Application.DTOs.Post;
using TravelPlanner.Application.DTOs.Response;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Models;

namespace TravelPlanner.Application.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsResponseDto> GetAnalytics(AnalyticsRequestDto request);
        Task LogEvent(string userId, string eventName, object eventData = null);
        Task LogUserActivity(string userId, string activityType, string description, object metadata = null);
        Task LogSearch(string userId, string query, string searchType, object filters = null);
        Task<AnalyticsData> GetSystemAnalytics(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, object>> GetUserAnalytics(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, object>> GetTripAnalytics(int tripId);
        Task<bool> GenerateReport(string userId, AnalyticsRequestDto request, string format = "pdf");
        Task<List<AnalyticsEvent>> GetRecentEvents(int limit = 100);
    }
}