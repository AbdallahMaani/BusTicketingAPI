using Bus_ticketingAPI.Entities;
using Bus_ticketing_Backend.DTOs;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId);
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);
        Task<Booking?> CreateBookingTransactionAsync(Guid userId, CreateBookingDto dto);
        Task<Booking?> UpdateBookingStatusAsync(Guid bookingId, Guid userId, bool isAdmin, string newStatus);
        Task<Booking?> UpdateBookingQuantityAsync(Guid bookingId, Guid userId, bool isAdmin, int newQuantity);
    }
}