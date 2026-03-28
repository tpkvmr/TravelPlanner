using System;
using System.Collections.Generic;

namespace TravelPlanner.Application.DTOs.Response
{
    public class AnalyticsResponseDto
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public Dictionary<string, object> Summary { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, ChartDataDto> Charts { get; set; } = new Dictionary<string, ChartDataDto>();

        public Dictionary<string, TrendDataDto> Trends { get; set; } = new Dictionary<string, TrendDataDto>();

        public Dictionary<string, List<TopItemDto>> TopLists { get; set; } = new Dictionary<string, List<TopItemDto>>();

        public List<AnalyticsRecommendationDto> Recommendations { get; set; } = new List<AnalyticsRecommendationDto>();

        public object RawData { get; set; }
    }

    public class ChartDataDto
    {
        public string Type { get; set; } 
        public List<string> Labels { get; set; } = new List<string>();
        public List<object> Datasets { get; set; } = new List<object>();
    }

    public class TrendDataDto
    {
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public List<decimal> Values { get; set; } = new List<decimal>();
        public decimal ChangePercentage { get; set; }
        public string TrendDirection { get; set; } 
    }

    public class TopItemDto
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public int Rank { get; set; }
        public decimal Change { get; set; }
    }

    public class AnalyticsRecommendationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } 
        public string Priority { get; set; } 
        public List<string> Actions { get; set; } = new List<string>();
    }
}