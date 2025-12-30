using Bus_ticketingAPI.Models;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface ITripRepository
    {
        Task<Trip> GetTripByIdAsync(Guid tripId);
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task AddTripAsync(Trip trip);
        Task UpdateTripAsync(Trip trip);
        Task DeleteTripAsync(Guid tripId);
    }
}
