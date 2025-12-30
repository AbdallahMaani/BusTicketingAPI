using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Models;
using Bus_ticketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class BusRepository : IBusRepository
    {
        private readonly AppDbContext _context;
        public BusRepository(AppDbContext context) => _context = context;

        public async Task<Bus> GetBusByIdAsync(Guid busId) =>
            await _context.Buses.FindAsync(busId);

        public async Task<IEnumerable<Bus>> GetAllBusesAsync() =>
            await _context.Buses.ToListAsync();

        public async Task AddBusAsync(Bus bus)
        {
            await _context.Buses.AddAsync(bus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBusAsync(Bus bus)
        {
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBusAsync(Guid busId)
        {
            var bus = await _context.Buses.FindAsync(busId);
            if (bus != null)
            {
                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();
            }
        }
    }
}