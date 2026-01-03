using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IBusRepository _repository;
        public BusController(IBusRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusDto>>> GetAll()
        {
            var items = await _repository.GetAllBusesAsync();
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<BusDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetBusByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(MapToDto(item));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBusDto dto)
        {
            var bus = new Bus
            {
                Operator = dto.Operator,
                Type = dto.Type,
                Capacity = dto.Capacity,
                Model = dto.Model,
                ModelYear = dto.ModelYear,
                DriverName = dto.DriverName,
                Features = dto.Features
            };

            await _repository.AddBusAsync(bus);

            // Fix: Return the mapped object so the admin sees what was created immediately
            return CreatedAtAction(nameof(GetById), new { id = bus.BusId }, MapToDto(bus));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBusDto dto)
        {
            var existing = await _repository.GetBusByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Operator = dto.Operator ?? existing.Operator;
            existing.Type = dto.Type ?? existing.Type;
            existing.Capacity = dto.Capacity != 0 ? dto.Capacity : existing.Capacity;
            existing.Model = dto.Model ?? existing.Model;
            existing.ModelYear = dto.ModelYear != 0 ? dto.ModelYear : existing.ModelYear;
            existing.DriverName = dto.DriverName ?? existing.DriverName;
            existing.Features = dto.Features ?? existing.Features;

            await _repository.UpdateBusAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteBusAsync(id);
            return NoContent();
        }

        private static BusDto MapToDto(Bus b)
        {
            return new BusDto
            {
                BusId = b.BusId,
                Operator = b.Operator,
                Type = b.Type,
                Capacity = b.Capacity,
                Model = b.Model,
                ModelYear = b.ModelYear,
                DriverName = b.DriverName,
                Features = b.Features
            };
        }
    }
}