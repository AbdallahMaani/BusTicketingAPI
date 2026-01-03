using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface ITripRepository
    {
        Task<Trip> GetTripByIdAsync(Guid tripId);
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task AddTripAsync(Trip trip);
        Task UpdateTripAsync(Trip trip);
        Task<IEnumerable<Trip>> SearchTripsAsync(string originCityId, string destinationCityId, DateTime? date);
        Task<IEnumerable<Trip>> FilterTripsAsync(string? originCityName,string? destinationCityName,DateTime? departureDate,string? busFeature,string? sortBy);
        Task<bool> DeleteTripAsync(Guid tripId);
    }
}
