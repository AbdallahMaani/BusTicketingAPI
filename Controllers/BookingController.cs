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
        [HttpPut("{id:Guid}/quantity")]
        public async Task<ActionResult<BookingDto>> UpdateBookingQuantity([FromRoute] Guid id, [FromBody] UpdateBookingQuantityDto dto)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var updated = await _repository.UpdateBookingQuantityAsync(id, userId, isAdmin, dto.Quantity);

            if (updated == null)
            {
                return BadRequest("Unable to update booking quantity. Ensure the booking is confirmed, you have sufficient balance, and the trip has available seats.");
            }

            return Ok(MapToDto(updated));
        }

        [Authorize]
        [HttpPut("{id:Guid}/status")]
        public async Task<ActionResult<BookingDto>> UpdateBookingStatus([FromRoute] Guid id, [FromBody] UpdateBookingStatusDto dto)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var updated = await _repository.UpdateBookingStatusAsync(id, userId, isAdmin, dto.Status);

            if (updated == null)
            {
                return BadRequest("Unable to update booking status. Ensure valid status transition or you have sufficient balance/seats.");
            }

            return Ok(MapToDto(updated));
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