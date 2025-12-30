using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Models;
using Bus_ticketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext _context;
        public StationRepository(AppDbContext context) => _context = context;

        public async Task<Station> GetStationByIdAsync(Guid stationId) =>
            await _context.Stations.FindAsync(stationId);

        public async Task<IEnumerable<Station>> GetAllStationsAsync() =>
            await _context.Stations.ToListAsync();

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