using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class RoutesRepository : IRoutesRepository
    {
        private readonly AppDbContext _context;

        public RoutesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Routes> GetRouteByIdAsync(Guid routeId)
        {
            return await _context.Routes
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }

        public async Task<IEnumerable<Routes>> GetAllRoutesAsync()
        {
            return await _context.Routes.AsNoTracking().ToListAsync();
        }
    }
}