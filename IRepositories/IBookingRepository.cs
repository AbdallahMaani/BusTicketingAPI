using Bus_ticketingAPI.Entities;
using Bus_ticketing_Backend.DTOs;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IBookingRepository
    {
        // specific data fetch
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId);
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);

        // Transactional Operations
        Task<Booking?> CreateBookingTransactionAsync(Guid userId, CreateBookingDto dto);
        Task<bool> CancelBookingTransactionAsync(Guid bookingId, Guid userId, bool isAdmin);

        // Admin Maintenance
        Task UpdateBookingAsync(Booking booking);
    }
}