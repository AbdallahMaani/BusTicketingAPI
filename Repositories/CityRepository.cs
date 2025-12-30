using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Models;
using Bus_ticketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;
        public CityRepository(AppDbContext context) => _context = context;

        public async Task<City> GetCityByIdAsync(string cityId)
        {
            return await _context.Cities.FindAsync(cityId);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync() =>
            await _context.Cities.ToListAsync();
    }
}