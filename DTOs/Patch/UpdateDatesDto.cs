namespace TravelPlanner.Application.DTOs.Patch
{
    public record UpdateDatesDto(DateTimeOffset? StartDate, DateTimeOffset? EndDate);
}
