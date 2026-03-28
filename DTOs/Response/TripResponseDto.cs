namespace TravelPlanner.Application.DTOs.Response
{
    public class TripResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Location { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public decimal? Budget { get; set; }
        public string? Category { get; set; }
        public List<string> Activities { get; set; } = new List<string>();
        public double? AverageRating { get; set; }
        public int? DurationDays { get; set; }
        public bool? IsAccessible { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public int ViewCount { get; set; }
        public int? TripDayIndex { get; set; }
        public string? Status { get; set; }
    }
}