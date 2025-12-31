using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;

        // Inject the repository through the constructor
        public CityController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _cityRepository.GetAllCitiesAsync();
            return Ok(cities);
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(string id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);

            if (city == null)
            {
                return NotFound(new { message = $"City with ID {id} not found." });
            }

            return Ok(city);
        }
    }
}