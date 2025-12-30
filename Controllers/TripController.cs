using System.Linq;
using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripRepository _repository;
        public TripController(ITripRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
        {
            var items = await _repository.GetAllTripsAsync();
            var result = items.Select(t => new TripDto
            {
                TripId = t.TripId,
                RouteId = t.RouteId,
                BusId = t.BusId,
                DepartureDate = t.DepartureDate,
                DepartureTime = t.DepartureTime,
                AvailableSeats = t.AvailableSeats,
                PriceJod = t.PriceJod,
                Status = t.Status
            });
            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<TripDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetTripByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new TripDto
            {
                TripId = item.TripId,
                RouteId = item.RouteId,
                BusId = item.BusId,
                DepartureDate = item.DepartureDate,
                DepartureTime = item.DepartureTime,
                AvailableSeats = item.AvailableSeats,
                PriceJod = item.PriceJod,
                Status = item.Status
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTripDto dto)
        {
            var trip = new Trip
            {
                RouteId = dto.RouteId,
                BusId = dto.BusId,
                DepartureDate = dto.DepartureDate,
                DepartureTime = dto.DepartureTime,
                AvailableSeats = dto.AvailableSeats,
                PriceJod = dto.PriceJod,
                Status = dto.Status
            };

            await _repository.AddTripAsync(trip);
            return CreatedAtAction(nameof(GetById), new { id = trip.TripId }, null);
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTripDto dto)
        {
            var existing = await _repository.GetTripByIdAsync(id);
            if (existing == null) return NotFound();

            existing.DepartureDate = dto.DepartureDate != default ? dto.DepartureDate : existing.DepartureDate;
            existing.DepartureTime = dto.DepartureTime != default ? dto.DepartureTime : existing.DepartureTime;
            existing.AvailableSeats = dto.AvailableSeats != 0 ? dto.AvailableSeats : existing.AvailableSeats;
            existing.PriceJod = dto.PriceJod != 0 ? dto.PriceJod : existing.PriceJod;
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateTripAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteTripAsync(id);
            return NoContent();
        }
    }
}