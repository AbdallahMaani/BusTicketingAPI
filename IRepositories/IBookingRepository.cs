using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IBookingRepository
    {
        Task<Booking> GetBookingByIdAsync(Guid bookingId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task AddBookingAsync(Booking booking);

        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(Guid bookingId);
    }
}
