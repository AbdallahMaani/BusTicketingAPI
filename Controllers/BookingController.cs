using System.Linq;
using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repository;
        public BookingController(IBookingRepository repository) => _repository = repository;

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
                Status = b.Status,
                PricePaid = b.PricePaid
            });
            return Ok(result);
        }

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
                Status = item.Status,
                PricePaid = item.PricePaid
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBookingDto dto)
        {
            var booking = new Booking
            {
                UserId = dto.UserId,
                TripId = dto.TripId,
                BookingDate = dto.BookingDate,
                PricePaid = dto.PricePaid,
                Status = dto.Status
            };

            await _repository.AddBookingAsync(booking);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, null);
            // this line returns 201 and location header the location header points to the newly created resource
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookingDto dto)
        {
            var existing = await _repository.GetBookingByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Status = dto.Status ?? existing.Status; // ?? means if dto.Status is null, keep existing.Status 

            await _repository.UpdateBookingAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteBookingAsync(id);
            return NoContent();
        }
    }
}