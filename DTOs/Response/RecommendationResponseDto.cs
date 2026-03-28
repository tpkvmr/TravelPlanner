using System;
using System.Collections.Generic;

namespace TravelPlanner.Application.DTOs.Response
{
    public class RecommendationResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal ConfidenceScore { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal? EstimatedBudget { get; set; }
        public string? Location { get; set; }

        public string? Category { get; set; }
        public double? Rating { get; set; }
        public string? Address { get; set; }

        public DateTime GeneratedAt { get; set; }
        public bool IsPersonalized { get; set; }
    }
}