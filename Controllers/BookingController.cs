using Bus_ticketing_Backend.Data;
using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repository;
        private readonly AppDbContext _context;
        public BookingController(IBookingRepository repository, AppDbContext appDbContext) 
        { _repository = repository;
          _context = appDbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var items = await _repository.GetAllBookingsAsync();
            var result = items.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                TripId = b.TripId,
                BookingDate = b.BookingDate,
                bookingStatus = b.bookingStatus,
                PriceTotal = b.PriceTotal,
                Quantity = b.Quantity
            });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<BookingDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetBookingByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new BookingDto
            {
                BookingId = item.BookingId,
                UserId = item.UserId,
                TripId = item.TripId,
                BookingDate = item.BookingDate,
                bookingStatus = item.bookingStatus,
                PriceTotal = item.PriceTotal,
                Quantity = item.Quantity
            };

            return Ok(dto);
        }

        // Add user-specific authorization checks:
        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<ActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Bookings
                .Where(b => b.UserId == Guid.Parse(userId))
                .ToListAsync();
            return Ok(bookings);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBookingWithTransactionAsync([FromBody] CreateBookingDto dto)
        {
            // Extract userId from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Invalid or missing user ID in token");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get trip with lock to prevent race conditions
                var trip = await _context.Trips
                    .Where(t => t.TripId == dto.TripId && t.tripStatus == "Scheduled")
                    .FirstOrDefaultAsync();

                if (dto.TripId == Guid.Empty)
                    return BadRequest("Trip ID is required");

                if (trip == null)
                    return BadRequest("Trip not found or not available");

                if (trip.AvailableSeats < dto.Quantity)
                    return BadRequest("Not enough seats available");

                // Check user balance
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                    return BadRequest("User not found");

                var totalPrice = trip.PriceJod * dto.Quantity;

                if (user.Balance < totalPrice)
                    return BadRequest("Insufficient balance");

                // Create booking
                var booking = new Booking
                {
                    UserId = userId,
                    TripId = dto.TripId,
                    BookingDate = DateTime.UtcNow,
                    bookingStatus = "Confirmed",
                    PriceTotal = totalPrice,
                    Quantity = dto.Quantity,
                };

                // Update trip seats
                trip.AvailableSeats -= dto.Quantity;

                // Deduct from user balance
                user.Balance -= totalPrice;

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(booking);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookingDto dto)
        {
            var existing = await _repository.GetBookingByIdAsync(id);
            if (existing == null) return NotFound();

            existing.bookingStatus = dto.bookingStatus ?? existing.bookingStatus; // ?? means if dto.Status is null, keep existing.Status 

            await _repository.UpdateBookingAsync(existing);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteBookingAsync(id);
            return NoContent();
        }
    }
}