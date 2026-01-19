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
                .AsNoTracking()
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
                var trip = await _context.Trips
                .Include(t => t.Route).ThenInclude(r => r.Origin)      
                .Include(t => t.Route).ThenInclude(r => r.Destination) 
                .Include(t => t.Bus)                                   
                .FirstOrDefaultAsync(t => t.TripId == dto.TripId && t.tripStatus == "Scheduled");

                if (trip == null || trip.AvailableSeats < dto.Quantity)
                    return null; 

                var user = await _context.Users.FindAsync(userId);
                var totalPrice = trip.PriceJod * dto.Quantity;

                if (user == null || user.Balance < totalPrice)
                    return null; 

                // Create Record
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

                
                trip.AvailableSeats -= dto.Quantity;
                user.Balance -= totalPrice;

                // with this link so the response JSON has data
                booking.Trip = trip;

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // this line means that the changes made during the transaction are saved permanently to the database.
                return booking;
            }
            catch
            {
                await transaction.RollbackAsync(); // this line means that if any error occurs during the transaction, all changes made during the transaction are undone, and the database is returned to its previous state.
                throw;
            }
        }

        public async Task<Booking?> UpdateBookingStatusAsync(Guid bookingId, Guid userId, bool isAdmin, string newStatus)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                if (booking == null) return null;
                if (!isAdmin && booking.UserId != userId) return null;
                if (booking.bookingStatus == newStatus) return booking; // Already in desired status

                var validStatuses = new[] { "Confirmed", "Cancelled" };
                if (!validStatuses.Contains(newStatus)) return null;

                var user = await _context.Users.FindAsync(booking.UserId);
                var trip = await _context.Trips.FindAsync(booking.TripId);

                if (user == null || trip == null) return null;

                // Handle status transition
                if (booking.bookingStatus == "Confirmed" && newStatus == "Cancelled")
                {
                    // Cancelling: refund user, return seats
                    user.Balance += booking.PriceTotal;
                    trip.AvailableSeats += booking.Quantity;
                }
                else if (booking.bookingStatus == "Cancelled" && newStatus == "Confirmed")
                {
                    // Re-confirming: deduct from user, take seats
                    if (user.Balance < booking.PriceTotal || trip.AvailableSeats < booking.Quantity)
                        return null; // Insufficient balance or seats

                    user.Balance -= booking.PriceTotal;
                    trip.AvailableSeats -= booking.Quantity;
                }

                booking.bookingStatus = newStatus;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Reload booking with related entities
                return await GetBookingByIdAsync(bookingId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Booking?> UpdateBookingQuantityAsync(Guid bookingId, Guid userId, bool isAdmin, int newQuantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                if (booking == null) return null;
                if (!isAdmin && booking.UserId != userId) return null;
                if (booking.bookingStatus != "Confirmed") return null; // Can only update confirmed bookings
                if (newQuantity < 1 || newQuantity > 10) return null; // Validate quantity

                var user = await _context.Users.FindAsync(booking.UserId);
                var trip = await _context.Trips.FindAsync(booking.TripId);

                if (user == null || trip == null) return null;

                var oldQuantity = booking.Quantity;
                var quantityDifference = newQuantity - oldQuantity;
                var oldPrice = booking.PriceTotal;
                var newPrice = trip.PriceJod * newQuantity;
                var priceDifference = newPrice - oldPrice;

                // Check if user has sufficient balance and trip has sufficient seats
                if (quantityDifference > 0)
                {
                    // Increasing quantity: need more seats and money
                    if (trip.AvailableSeats < quantityDifference)
                        return null; // Not enough seats

                    if (user.Balance < priceDifference)
                        return null; // Not enough balance

                    trip.AvailableSeats -= quantityDifference;
                    user.Balance -= priceDifference;
                }
                else if (quantityDifference < 0)
                {
                    // Decreasing quantity: refund seats and money
                    trip.AvailableSeats -= quantityDifference; // Negative value, so adds seats
                    user.Balance -= priceDifference; // Negative value, so adds balance
                }

                booking.Quantity = newQuantity;
                booking.PriceTotal = newPrice;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Reload booking with related entities
                return await GetBookingByIdAsync(bookingId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}