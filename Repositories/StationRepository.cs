using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext _context;
        public StationRepository(AppDbContext context) => _context = context;

        public async Task<Station> GetStationByIdAsync(Guid stationId)
        {
            return await _context.Stations
                .Include(s => s.City) 
                .FirstOrDefaultAsync(s => s.Id == stationId);
        }

        public async Task<IEnumerable<Station>> GetAllStationsAsync()
        {
            return await _context.Stations
                .Include(s => s.City) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddStationAsync(Station station)
        {
            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStationAsync(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStationAsync(Guid stationId)
        {
            var station = await _context.Stations.FindAsync(stationId);
            if (station != null)
            {
                _context.Stations.Remove(station);
                await _context.SaveChangesAsync();
            }
        }
    }
}