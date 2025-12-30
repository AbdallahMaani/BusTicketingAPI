using System.Linq;
using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Models;
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
            var result = items.Select(r => new RouteDto
            {
                RouteId = r.RouteId,
                OriginId = r.OriginId,
                DestinationId = r.DestinationId,
                DistanceKm = r.DistanceKm,
                DurationHrs = r.DurationHrs
            });
            return Ok(result);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<RouteDto>> GetById([FromRoute] Guid id)
        {
            var item = await _repository.GetRouteByIdAsync(id);
            if (item == null) return NotFound();

            var dto = new RouteDto
            {
                RouteId = item.RouteId,
                OriginId = item.OriginId,
                DestinationId = item.DestinationId,
                DistanceKm = item.DistanceKm,
                DurationHrs = item.DurationHrs
            };

            return Ok(dto);
        }
    }
}