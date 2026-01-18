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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TripDto>>> Search(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime? date)
        {
            var items = await _repository.SearchTripsAsync(from, to, date);
            return Ok(items.Select(MapToDto));
        }

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

            var trip = new Trip
            {
                TripId = Guid.NewGuid(),
                RouteId = dto.RouteId,
                BusId = dto.BusId,
                OriginStationId = dto.OriginStationId,
                DestinationStationId = dto.DestinationStationId,
                DepartureDate = dto.DepartureDate,
                DepartureTime = dto.DepartureTime,
                AvailableSeats = dto.AvailableSeats,
                PriceJod = dto.PriceJod,
                tripStatus = dto.Status ?? "Scheduled"
            };

            await _repository.AddTripAsync(trip);

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

        private static TripDto MapToDto(Trip t)
        {
            return new TripDto
            {
                TripId = t.TripId,
                RouteId = t.RouteId,
                BusId = t.BusId,
                DepartureDate = t.DepartureDate,
                DepartureTime = t.DepartureTime,
                AvailableSeats = t.AvailableSeats,
                PriceJod = t.PriceJod,
                Status = t.tripStatus,

                // Route City Names
                OriginName = t.Route?.Origin?.NameEn ?? "Unknown",
                DestinationName = t.Route?.Destination?.NameEn ?? "Unknown",

                // Bus Info
                BusType = t.Bus?.Type ?? "Standard",
                DriverName = t.Bus?.DriverName ?? "Unknown",
                Features = t.Bus?.Features ?? "Not Listed",

                // ORIGIN STATION (Departure)
                OriginStationId = t.OriginStationId,
                OriginStationName = t.OriginStation?.StationName ?? "Unknown",
                OriginStationNameEn = t.OriginStation?.NameEn ?? "Unknown",
                OriginStationStreet = t.OriginStation?.StreetEn ?? "Unknown",
                OriginStationLat = t.OriginStation?.Lat,
                OriginStationLng = t.OriginStation?.Lng,

                // DESTINATION STATION (Arrival)
                DestinationStationId = t.DestinationStationId,
                DestinationStationName = t.DestinationStation?.StationName ?? "Unknown",
                DestinationStationNameEn = t.DestinationStation?.NameEn ?? "Unknown",
                DestinationStationStreet = t.DestinationStation?.StreetEn ?? "Unknown",
                DestinationStationLat = t.DestinationStation?.Lat,
                DestinationStationLng = t.DestinationStation?.Lng
            };

        }
    }
}