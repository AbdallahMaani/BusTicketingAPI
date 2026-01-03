using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationRepository _repository;
        public StationController(IStationRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationDto>>> GetAll()
        {
            var items = await _repository.GetAllStationsAsync();
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<StationDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetStationByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(MapToDto(item));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateStationDto dto)
        {
            var station = new Station
            {
                CityId = dto.CityId,
                NameEn = dto.NameEn,
                StationName = dto.StationName,
                StreetEn = dto.StreetEn,
                Lat = dto.Lat,
                Lng = dto.Lng
            };

            await _repository.AddStationAsync(station);
            // Return mapped object
            return CreatedAtAction(nameof(GetById), new { id = station.Id }, MapToDto(station));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateStationDto dto)
        {
            var existing = await _repository.GetStationByIdAsync(id);
            if (existing == null) return NotFound();

            existing.NameEn = dto.NameEn ?? existing.NameEn;
            existing.StationName = dto.StationName ?? existing.StationName;
            existing.StreetEn = dto.StreetEn ?? existing.StreetEn;
            existing.Lat = dto.Lat ?? existing.Lat;
            existing.Lng = dto.Lng ?? existing.Lng;

            await _repository.UpdateStationAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteStationAsync(id);
            return NoContent();
        }

        // --- HELPER METHOD ---
        private static StationDto MapToDto(Station s)
        {
            return new StationDto
            {
                Id = s.Id,
                CityId = s.CityId,
                NameEn = s.NameEn,
                StationName = s.StationName,
                StreetEn = s.StreetEn,
                Lat = s.Lat,
                Lng = s.Lng
            };
        }
    }
}