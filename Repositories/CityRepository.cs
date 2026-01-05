using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;
        public CityRepository(AppDbContext context) => _context = context;

        public async Task<City> GetCityByIdAsync(string cityId)
        {
            return await _context.Cities
                .Include(c => c.busStations) 
                .FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _context.Cities
                .Include(c => c.busStations)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}