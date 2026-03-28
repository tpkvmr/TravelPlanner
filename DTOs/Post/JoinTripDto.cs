using TravelPlanner.Core.Enums;

namespace TravelPlanner.Application.DTOs.Post
{
    public record JoinTripDto(int TripId, UserTripRole Role);
}
