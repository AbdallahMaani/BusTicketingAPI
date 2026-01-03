using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripRepository _repository;
        public TripController(ITripRepository repository) => _repository = repository;

        // NEW: Search Endpoint (Public) - Search by City IDs and Date
        // usage: api/trip/search?from=LOC_AMN&to=LOC_IRD&date=2025-12-31
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TripDto>>> Search(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime? date)
        {
            var items = await _repository.SearchTripsAsync(from, to, date);
            return Ok(items.Select(MapToDto));
        }

        // NEW: Advanced Filter Endpoint (Public) - Filter by City Names, Bus Features, and Sort
        // usage: api/trip/filters?originCityName=Amman&destinationCityName=Irbid&departureDate=2025-12-31&busFeature=WiFi&sortBy=price_asc
        // sortBy options: departure_time, available_seats, price_asc, price_desc (default: departure date desc)
        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<TripDto>>> Filters([FromQuery] string? originCityName,[FromQuery] string? destinationCityName,[FromQuery] DateTime? departureDate,[FromQuery] string? busFeature,[FromQuery] string? sortBy)
        {
            var items = await _repository.FilterTripsAsync(originCityName,destinationCityName,departureDate,busFeature,sortBy);
            
            return Ok(items.Select(MapToDto));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
        {
            var items = await _repository.GetAllTripsAsync();
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<TripDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetTripByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(MapToDto(item));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTripDto dto)
        {
            // Optional: Add validation here to check if Bus is free at this time

            var trip = new Trip
            {
                TripId = Guid.NewGuid(),
                RouteId = dto.RouteId,
                BusId = dto.BusId,
                DepartureDate = dto.DepartureDate,
                DepartureTime = dto.DepartureTime,
                AvailableSeats = dto.AvailableSeats,
                PriceJod = dto.PriceJod,
                tripStatus = dto.Status ?? "Scheduled"
            };

            await _repository.AddTripAsync(trip);

            // Fix: Return the mapped DTO, not null
            return CreatedAtAction(nameof(GetById), new { id = trip.TripId }, MapToDto(trip));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTripDto dto)
        {
            var existing = await _repository.GetTripByIdAsync(id);
            if (existing == null) return NotFound();

            existing.DepartureDate = dto.DepartureDate != default ? dto.DepartureDate : existing.DepartureDate;
            existing.DepartureTime = dto.DepartureTime != default ? dto.DepartureTime : existing.DepartureTime;
            existing.AvailableSeats = dto.AvailableSeats != 0 ? dto.AvailableSeats : existing.AvailableSeats;
            existing.PriceJod = dto.PriceJod != 0 ? dto.PriceJod : existing.PriceJod;
            existing.tripStatus = dto.Status ?? existing.tripStatus;

            await _repository.UpdateTripAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var success = await _repository.DeleteTripAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        // Helper: Maps Entity to Rich DTO
        private static TripDto MapToDto(Trip t)
        {
            return new TripDto
            {
                TripId = t.TripId,
                RouteId = t.RouteId,
                // Null checks in case Includes weren't used or data is missing
                OriginName = t.Route?.Origin?.NameEn ?? "Unknown",
                DestinationName = t.Route?.Destination?.NameEn ?? "Unknown",
                BusId = t.BusId,
                BusType = t.Bus?.Type ?? "Standard",
                DepartureDate = t.DepartureDate,
                DepartureTime = t.DepartureTime,
                AvailableSeats = t.AvailableSeats,
                PriceJod = t.PriceJod,
                Status = t.tripStatus
            };
        }
    }
}