using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities; // Ensure you have this namespace for the entity class
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IRoutesRepository _repository;
        public RoutesController(IRoutesRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
        {
            var items = await _repository.GetAllRoutesAsync();
            return Ok(items.Select(MapToDto));
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<RouteDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetRouteByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(MapToDto(item));
        }

        private static RouteDto MapToDto(Routes r)
        {
            return new RouteDto
            {
                RouteId = r.RouteId,
                OriginId = r.OriginId,
                DestinationId = r.DestinationId,
                DistanceKm = r.DistanceKm,
                DurationHrs = r.DurationHrs
            };
        }
    }
}