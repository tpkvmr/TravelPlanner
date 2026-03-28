using TravelPlanner.Core.Common;
using TravelPlanner.Core.Enums;
using TravelPlanner.Core.Interfaces.Repositories;
using TravelPlanner.Core.Interfaces.Services;
using TravelPlanner.Core.Models.Requests;
using TravelPlanner.Core.Models.Results;

namespace TravelPlanner.Application.Services
{
    public class TripAccessHelperService(ITripRepository tripRepository) : ITripAccessHelperService
    {
        public async Task<TripResult> GetTripWithAccessCheck(UserResourceIdRequest request)
        {
            var trip = await tripRepository.GetByIdAsync(request.ResourceId);

            if (trip == null) return new TripResult(false, null, false, Errors: [DomainErrors.General.NotFound]);

            if (!trip.UserTrips.Any(ut => ut.UserId == request.UserId))
                return new TripResult(false, null, false,  Errors: [DomainErrors.Trip.UserNotMember]);

            var isOwner = trip.UserTrips
               .Any(ut => ut.UserId == request.UserId && ut.Role == UserTripRole.Owner);


            return new TripResult(true,  trip, isOwner);
        }

        public async Task<bool> HasAccessToTrip(int tripId, string userId)
        {
            var trip = await GetTripWithAccessCheck(new(tripId, userId));

            return trip.Success;
        }
    }
}
