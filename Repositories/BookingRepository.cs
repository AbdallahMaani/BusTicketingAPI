using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        public BookingRepository(AppDbContext context) => _context = context;

        public async Task<Booking> GetBookingByIdAsync(Guid bookingId) =>
            await _context.Bookings.FindAsync(bookingId);

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync() =>
            await _context.Bookings.ToListAsync();

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            if (existingBooking == null)
                return;

            // تحديث الحقول المسموح تعديلها فقط
            existingBooking.Status = booking.Status;
            existingBooking.PricePaid = booking.PricePaid;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }
}