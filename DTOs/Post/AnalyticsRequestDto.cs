using System;

namespace TravelPlanner.Application.DTOs.Post
{
    public class AnalyticsRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string PeriodType { get; set; } = "Daily";

        public string UserId { get; set; }

        public int? TripId { get; set; }

        public string[] Metrics { get; set; } = new string[]
        {
            "UserActivity",
            "TripStatistics",
            "POIStatistics",
            "SearchAnalytics"
        };

        public bool IncludeCharts { get; set; } = true;
        public bool IncludeRawData { get; set; } = false;

        public string Format { get; set; } = "Json"; 
    }
}