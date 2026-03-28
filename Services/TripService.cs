using TravelPlanner.Core.Common;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Enums;
using TravelPlanner.Core.Interfaces.Repositories;
using TravelPlanner.Core.Interfaces.Services;
using TravelPlanner.Core.Models.Requests;
using TravelPlanner.Core.Models.Results;
using Microsoft.AspNetCore.Identity;

namespace TravelPlanner.Application.Services
{
    public class TripService(ITripRepository tripRepository, IUserRepository userRepository,
    UserManager<ApplicationUser> userManager, IPointOfInterestService pointOfInterestService, ITripAccessHelperService tripAccessHelperService) : ITripService
    {
        public async Task<TripIdResult> CreateAsync(CreateTripRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return new(false, 0, Errors: [DomainErrors.Auth.UserNotFound]);

            var trip = new Trip
            {
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var userTrip = new UserTrip
            {
                User = user,
                UserId = user.Id,
                Trip = trip,
                Role = UserTripRole.Owner
            };

            trip.UserTrips.Add(userTrip);

            await tripRepository.AddAsync(trip);

            return new TripIdResult(true, trip.Id);
        }

        public async Task<RequestResult> DeleteAsync(UserResourceIdRequest request)
        {
            var tripResult = await tripAccessHelperService.GetTripWithAccessCheck(request);
            if (!tripResult.Success || tripResult.Trip == null) return new RequestResult(false, Errors: tripResult.Errors);

            var isOwner = tripResult.Trip.UserTrips
                .Any(ut => ut.UserId == request.UserId && ut.Role == UserTripRole.Owner);

            if (!isOwner) return new(false, Errors: [DomainErrors.Trip.NotOwner]);

            await tripRepository.RemoveAsync(tripResult.Trip);

            return new RequestResult(true);
        }

        public async Task<RequestResult> LeaveAsync(UserResourceIdRequest request)
        {
            var tripResult = await tripAccessHelperService.GetTripWithAccessCheck(request);
            if (!tripResult.Success || tripResult.Trip == null) return new RequestResult(false, Errors: tripResult.Errors);

            var userTrip = tripResult.Trip.UserTrips.FirstOrDefault(ut => ut.UserId == request.UserId);
            if (userTrip == null) return new(false, Errors: [DomainErrors.Trip.UserNotMember]);

            if (userTrip.Role == UserTripRole.Owner)
            {
                return new(false, Errors: [DomainErrors.Trip.OwnerCannotLeave]);
            }

            tripResult.Trip.UserTrips.Remove(userTrip);
            await tripRepository.SaveChangesAsync();

            return new RequestResult(true);
        }

        public async Task<TripsListResult> GetAllAsync(string userId)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null) return new(false, [], Errors: [DomainErrors.Auth.UserNotFound]);

            List<Trip> trips = [];

            foreach (var item in user.UserTrips)
            {
                var trip = await tripRepository.GetByIdAsync(item.Trip.Id);
                if (trip == null) continue;

                var pois = await pointOfInterestService.GetAllAsync(new UserResourceIdRequest(trip.Id, userId));

                if(pois.Data != null)
                {
                    trip.PointsOfInterest = (List<PointOfInterest>)pois.Data;
                }

                trips.Add(trip);
            }

            return new(true, trips);
        }

        public async Task<TripResult> GetByIdAsync(UserResourceIdRequest request)
        {
            var trip = await tripRepository.GetByIdAsync(request.ResourceId);

            if (trip == null) return new(false, null, false, Errors: [DomainErrors.General.NotFound]);

            if (!trip.UserTrips.Any(ut => ut.UserId == request.UserId))
                return new(false, null, false, Errors: [DomainErrors.Trip.UserNotMember]);

            var userTrip = trip.UserTrips.First(ut => ut.UserId == request.UserId);

            var pois = await pointOfInterestService.GetAllAsync(new UserResourceIdRequest(trip.Id, request.UserId));

            if (pois.Data != null)
            {
                trip.PointsOfInterest = (List<PointOfInterest>)pois.Data;
            }

            var isOwner = trip.UserTrips
               .Any(ut => ut.UserId == request.UserId && ut.Role == UserTripRole.Owner);

            return new TripResult(true, trip, isOwner);
        }

        public async Task<TripIdResult> JoinAsync(JoinTripRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return new(false, null, Errors: [DomainErrors.Auth.UserNotFound]);

            var trip = await tripRepository.GetByIdAsync(request.TripId);

            if (trip == null) return new(false, null, Errors: [DomainErrors.General.NotFound]);

            if (trip.UserTrips.Any(ut => ut.UserId == user.Id))
                return (TripIdResult)new(false, null, Errors: [DomainErrors.Trip.AlreadyMember]);

            var userTrip = new UserTrip
            {
                User = user,
                UserId = user.Id,
                Trip = trip,
                Role = request.Role
            };

            await tripRepository.AddAsync(trip);

            return new TripIdResult(true, trip.Id);
        }

        public async Task<RequestResult> UpdateDatesAsync(UpdateDatesRequest request)
        {
            var tripResult = await tripAccessHelperService.GetTripWithAccessCheck(new(request.Id, request.UserId));
            if (!tripResult.Success || tripResult.Trip == null) return new RequestResult(false, Errors: tripResult.Errors);

            tripResult.Trip.StartDate = request.StartDate;
            tripResult.Trip.EndDate = request.EndDate;

            await tripRepository.UpdateDatesAsync(request);

            return new RequestResult(true, new { tripResult.Trip.Id, tripResult.Trip.StartDate, tripResult.Trip.EndDate });
        }

        public async Task<RequestResult> UpdateDescriptionAsync(UpdateTextRequest request)
        {
            var tripResult = await tripAccessHelperService.GetTripWithAccessCheck(new(request.Id, request.UserId));
            if (!tripResult.Success || tripResult.Trip == null) return new RequestResult(false, Errors: tripResult.Errors);

            tripResult.Trip.Description = request.NewText;
            await tripRepository.SaveChangesAsync();
            return new RequestResult(true, new { tripResult.Trip.Id, tripResult.Trip.Description });
        }

        public async Task<RequestResult> UpdateTitleAsync(UpdateTextRequest request)
        {
            var tripResult = await tripAccessHelperService.GetTripWithAccessCheck(new(request.Id, request.UserId));
            if (!tripResult.Success || tripResult.Trip == null) return new RequestResult(false, Errors: tripResult.Errors);

            tripResult.Trip.Title = request.NewText;
            await tripRepository.SaveChangesAsync();
            return new RequestResult(true, new { tripResult.Trip.Id, tripResult.Trip.Title });
        }

        public async Task<RequestResult> SetBudgetAsync(SetTripBudgetRequest request)
        {
            var tripResult = await tripAccessHelperService
                .GetTripWithAccessCheck(new(request.TripId, request.UserId));

            if (!tripResult.Success || tripResult.Trip == null)
                return new RequestResult(false, Errors: tripResult.Errors);

            var trip = tripResult.Trip;

            if (request.TotalBudget < 0)
                return new RequestResult(false, Errors: [DomainErrors.General.ValidationFailed]
);

            trip.TotalBudget = request.TotalBudget;

            trip.DailyBudgets ??= [];

            trip.DailyBudgets.Clear();

            foreach (var day in request.DailyBudgets)
            {
                if (day.PlannedAmount < 0)
                    return new RequestResult(false, Errors: [DomainErrors.General.ValidationFailed]);

                trip.DailyBudgets.Add(new DailyBudget
                {
                    Date = day.Date,
                    PlannedAmount = day.PlannedAmount,
                    TripId = trip.Id
                });
            }

            await tripRepository.SaveChangesAsync();

            return new RequestResult(true);
        }

        Task<TripBudgetResult> ITripService.SetBudgetAsync(SetTripBudgetRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
