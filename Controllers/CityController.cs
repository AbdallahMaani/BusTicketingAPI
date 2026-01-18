using Bus_ticketing_Backend.DTOs;
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

        public CityController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
        {
            var cities = await _cityRepository.GetAllCitiesAsync();
            var result = cities.Select(MapToDto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(string id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);

            if (city == null)
            {
                return NotFound(new { message = $"City with ID {id} not found." });
            }

            return Ok(MapToDto(city));
        }

        private static CityDto MapToDto(City city)
        {
            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                busStations = city.busStations.Select(s => new StationDto
                {
                    Id = s.Id,
                    NameEn = s.NameEn,
                    StationName = s.StationName,
                    StreetEn = s.StreetEn,
                    Lat = s.Lat,
                    Lng = s.Lng
                }).ToList()
            };
        }
    }
}