using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repository;

        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var items = await _repository.GetAllBookingsAsync();
            var dtos = items.Select(MapToDto);
            return Ok(dtos);
        }

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings() // only current user and admin can see their bookings
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var bookings = await _repository.GetUserBookingsAsync(userId);
            var dtos = bookings.Select(MapToDto);
            return Ok(dtos);
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<BookingDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetBookingByIdAsync(id);
            if (item == null) return NotFound();

            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && item.UserId != userId)
            {
                return Forbid();
            }

            return Ok(MapToDto(item));
        }

        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateBooking([FromRoute] Guid id, [FromBody] UpdateBookingDto updateBookingDto)
        {
            var existing = await _repository.GetBookingByIdAsync(id);
            if (existing == null) return NotFound();

            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existing.UserId != userId)
            {
                return Forbid();
            }

            if (updateBookingDto.PriceTotal != 0)
                existing.PriceTotal = updateBookingDto.PriceTotal;

            if (updateBookingDto.Quantity != 0)
                existing.Quantity = updateBookingDto.Quantity;

            await _repository.UpdateBookingAsync(existing);
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            if (dto.TripId == Guid.Empty) return BadRequest("Trip ID is required");

            try
            {
                // Delegate logic to repository
                var result = await _repository.CreateBookingTransactionAsync(userId, dto);

                if (result == null)
                    return BadRequest("Booking failed. Ensure you have balance and the bus has seats.");

                return CreatedAtAction(nameof(GetById), new { id = result.BookingId }, MapToDto(result));
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error during booking.");
            }
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> CancelBooking([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var success = await _repository.CancelBookingTransactionAsync(id, userId, isAdmin);

            if (!success)
            {
                return BadRequest("Unable to cancel booking. It may be already cancelled, or you do not have permission.");
            }

            return Ok(new { message = "Booking cancelled and amount refunded." });
        }
        // will be the booked trip deleted from the DB ? the answer is NO then why we used httpdelete ?
        // Because we are following RESTful conventions where DELETE is used to indicate the removal of a resource.
        // The booking status will be changed to "Cancelled" and the amount will be refunded to the user balance but the record will remain in the DB for history purpose.

        [Authorize]
        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        private static BookingDto MapToDto(Booking b)
        {
            return new BookingDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                TripId = b.TripId,
                BookingDate = b.BookingDate,
                bookingStatus = b.bookingStatus,
                PriceTotal = b.PriceTotal,
                Quantity = b.Quantity,
                OriginName = b.Trip?.Route?.Origin?.NameEn ?? "Unknown",
                DestinationName = b.Trip?.Route?.Destination?.NameEn ?? "Unknown",
                BusType = b.Trip?.Bus?.Type ?? "Standard",
            };
        }
    }
}