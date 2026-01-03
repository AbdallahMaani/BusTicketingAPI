using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketing_Backend.DTOs;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync() 
        {
            return await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Origin)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Destination)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Bus).AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId) =>
            await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Origin)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Destination)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Bus)
                .AsNoTracking() // this line means that the returned entities are not tracked by the context, which can improve performance for read-only operations.
                .ToListAsync();

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId) =>
            await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Origin)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Route)
                        .ThenInclude(r => r.Destination)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Bus)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);


        public async Task<Booking?> CreateBookingTransactionAsync(Guid userId, CreateBookingDto dto)
        {
            // Start a transaction to ensure Money and Seats are consistent
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate Trip and Seats
                var trip = await _context.Trips
            .Include(t => t.Route).ThenInclude(r => r.Origin)      // <--- ADDED
            .Include(t => t.Route).ThenInclude(r => r.Destination) // <--- ADDED
            .Include(t => t.Bus)                                   // <--- ADDED
            .FirstOrDefaultAsync(t => t.TripId == dto.TripId && t.tripStatus == "Scheduled");

                if (trip == null || trip.AvailableSeats < dto.Quantity)
                    return null; // Trip full or invalid

                // 2. Validate User and Balance
                var user = await _context.Users.FindAsync(userId);
                var totalPrice = trip.PriceJod * dto.Quantity;

                if (user == null || user.Balance < totalPrice)
                    return null; // Insufficient funds

                // 3. Create Record
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    UserId = userId,
                    TripId = dto.TripId,
                    BookingDate = DateTime.UtcNow,
                    bookingStatus = "Confirmed",
                    PriceTotal = totalPrice,
                    Quantity = dto.Quantity
                };

                // 4. Update Balances (The Transaction Logic)
                trip.AvailableSeats -= dto.Quantity;
                user.Balance -= totalPrice;

                // CHANGE 2: I added this link so the response JSON has data
                booking.Trip = trip; // <--- ADDED

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                // 5. Commit
                await transaction.CommitAsync(); // this line means that the changes made during the transaction are saved permanently to the database.
                return booking;
            }
            catch
            {
                await transaction.RollbackAsync(); // this line means that if any error occurs during the transaction, all changes made during the transaction are undone, and the database is returned to its previous state.
                throw;
            }
        }

        public async Task<bool> CancelBookingTransactionAsync(Guid bookingId, Guid userId, bool isAdmin)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                // Validation: Exists? Already Cancelled? Is Owner or Admin?
                if (booking == null) return false;
                if (!isAdmin && booking.UserId != userId) return false;
                if (booking.bookingStatus == "Cancelled") return false;

                // Load related data
                var user = await _context.Users.FindAsync(booking.UserId);
                var trip = await _context.Trips.FindAsync(booking.TripId);

                // 1. Refund User
                if (user != null)
                {
                    user.Balance += booking.PriceTotal;
                }

                // 2. Restock Seats (Only if trip is not past/cancelled)
                if (trip != null)
                {
                    trip.AvailableSeats += booking.Quantity;
                }

                // 3. Update Status (Soft Delete)
                booking.bookingStatus = "Cancelled";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}