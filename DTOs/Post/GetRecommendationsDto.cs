using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Application.DTOs.Post
{
    public class GetRecommendationsDto
    {
        [Required]
        public string UserId { get; set; } = null!;

        public int? Limit { get; set; } = 10;

        public List<string> RecommendationTypes { get; set; } = new List<string>();

        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; } = 50;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal? MaxBudget { get; set; }

        public List<string> ExcludeIds { get; set; } = new List<string>();

        public bool IncludeExplanation { get; set; } = true;
    }
}