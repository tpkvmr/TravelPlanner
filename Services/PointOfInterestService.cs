using TravelPlanner.Core.Common;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Interfaces.Repositories;
using TravelPlanner.Core.Interfaces.Services;
using TravelPlanner.Core.Models.Requests;
using TravelPlanner.Core.Models.Results;

namespace TravelPlanner.Application.Services
{
    public class PointOfInterestService(IPointOfInterestRepository pointOfInterestRepository, ITripAccessHelperService tripAccessHelperService) : IPointOfInterestService
    {

        public async Task<RequestResult> CreateAsync(CreatePointOfInterestRequest request)
        {
            var poi = new PointOfInterest
            {
                Title = request.Title,
                Description = request.Description,
                Url = request.Url,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type,
                TripId = request.TripId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await pointOfInterestRepository.AddAsync(poi);

            return new RequestResult(true, null);
        }
        public async Task<RequestResult> UpdateAsync(UpdatePointOfInterestRequest request)
        {

            var poi = await pointOfInterestRepository.GetByIdAsync(request.PoiId);

            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new RequestResult(false, Errors: ["Forbidden"]);
            }


            poi.Title = request.Title;
            poi.Description = request.Description;
            poi.StartDate = request.StartDate;
            poi.EndDate = request.EndDate;
            poi.Url = request.Url;
            poi.Type = request.Type;
            poi.TripId = request.TripId;
            poi.Longitude = request.Longitude;
            poi.Latitude = request.Latitude;

            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> DeleteAsync(UserResourceIdRequest request)
        {
            var pointOfInterest = await pointOfInterestRepository.GetByIdAsync(request.ResourceId);

            if (pointOfInterest == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

           await pointOfInterestRepository.Remove(pointOfInterest);

           return new RequestResult(true, null);
        }

        public async Task<RequestResult> GetAllAsync(UserResourceIdRequest request)
        {
            var trip = await tripAccessHelperService.GetTripWithAccessCheck(request);

            var pois = await pointOfInterestRepository.GetAllAsync(request.ResourceId);

            return new RequestResult(true, pois);
        }

        public async Task<RequestResult> GetByIdAsync(UserResourceIdRequest request)
        {
            var trip = await tripAccessHelperService.GetTripWithAccessCheck(request);

            var pointOfInterest = await pointOfInterestRepository.GetByIdAsync(request.ResourceId);

            if (pointOfInterest == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            return new RequestResult(true, pointOfInterest);
        }

        public async Task<RequestResult> UpdateCoordinatesAsync(UpdateCoordinatesRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);

            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new RequestResult(false, Errors: ["Forbidden"]);
            }

            poi.Longitude = request.Longitude;
            poi.Latitude = request.Latitude;

            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateDatesAsync(UpdateDatesRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new(false, DomainErrors.General.Forbidden);
            }

            poi.StartDate = request.StartDate;
            poi.EndDate = request.EndDate;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateDescriptionAsync(UpdateTextRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new(false, DomainErrors.General.Forbidden);
            }

            poi.Description = request.NewText;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateTitleAsync(UpdateTextRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new(false, DomainErrors.General.Forbidden);
            }

            poi.Title = request.NewText;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateTripDayIndexAsync(UpdateTripDayIndexRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
            {
                return new(false, DomainErrors.General.NotFound);
            }

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
            {
                return new(false, DomainErrors.General.Forbidden);
            }

            poi.TripDayIndex = request.NewTripDayIndex;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateTypeAsync(UpdateTypeRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
                return new(false, DomainErrors.General.NotFound);

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
                return new(false, DomainErrors.General.Forbidden);

            poi.Type = request.NewType;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }

        public async Task<RequestResult> UpdateUrlAsync(UpdateTextRequest request)
        {
            var poi = await pointOfInterestRepository.GetByIdAsync(request.Id);
            if (poi == null)
                return new(false, DomainErrors.General.NotFound);

            if (!await tripAccessHelperService.HasAccessToTrip(poi.TripId, request.UserId))
                return new(false, DomainErrors.General.Forbidden);

            poi.Url = request.NewText;
            await pointOfInterestRepository.SaveChangesAsync();

            return new RequestResult(true, null);
        }
    }
}
