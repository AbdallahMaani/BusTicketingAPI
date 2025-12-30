using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Models;
using Bus_ticketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _context;

        public TripRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Trip> GetTripByIdAsync(Guid tripId)
        {
             // Include is often used here to get Route and Bus details with the Trip
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.TripId == tripId);
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            return await _context.Trips.ToListAsync();
        }

        public async Task AddTripAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTripAsync(Guid tripId)
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
            }
        }
    }
}