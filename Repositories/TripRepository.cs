using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly AppDbContext _context;

        public TripRepository(AppDbContext context) => _context = context;

        public async Task<Trip> GetTripByIdAsync(Guid tripId)
        {
            return await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                    .ThenInclude(r => r.Origin)
                .Include(t => t.Route)
                    .ThenInclude(r => r.Destination)
                .FirstOrDefaultAsync(t => t.TripId == tripId);
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            // Only return active/scheduled trips to regular users usually
            // But for Admin, we might want all. Here we fetch all with details.
            return await _context.Trips
                .Include(t => t.Route).ThenInclude(r => r.Origin)
                .Include(t => t.Route).ThenInclude(r => r.Destination)
                .OrderByDescending(t => t.DepartureDate).AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> SearchTripsAsync(string originId, string destId, DateTime? date)
        {
            var query = _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route).ThenInclude(r => r.Origin)
                .Include(t => t.Route).ThenInclude(r => r.Destination)
                .Where(t => t.tripStatus == "Scheduled" && t.AvailableSeats > 0);

            // Filter by Route
            if (!string.IsNullOrEmpty(originId) && !string.IsNullOrEmpty(destId))
            {
                query = query.Where(t => t.Route.OriginId == originId && t.Route.DestinationId == destId);
            }

            // Filter by Date (Optional)
            if (date.HasValue)
            {
                // Compare only the Date part, ignore time
                query = query.Where(t => t.DepartureDate == DateOnly.FromDateTime(date.Value));
            }

            return await query.ToListAsync();
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

        // IMPROVED: Safe Delete
        public async Task<bool> DeleteTripAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.Bookings) // Check for bookings
                .FirstOrDefaultAsync(t => t.TripId == tripId);

            if (trip == null) return false;

            // If people have booked, DO NOT delete. Cancel instead.
            if (trip.Bookings.Any())
            {
                trip.tripStatus = "Cancelled"; // Soft Delete
            }
            else
            {
                _context.Trips.Remove(trip); // Hard Delete (Safe because empty)
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Trip>> FilterTripsAsync(string? originCityName,string? destinationCityName,DateTime? departureDate,string? busFeature,string? sortBy)
        {
            var query = _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                    .ThenInclude(r => r.Origin)
                .Include(t => t.Route)
                    .ThenInclude(r => r.Destination)
                .Where(t => t.tripStatus == "Scheduled" && t.AvailableSeats > 0);

            // Filter by Origin City Name (case-insensitive)
            if (!string.IsNullOrWhiteSpace(originCityName))
            {
                query = query.Where(t => t.Route.Origin.NameEn.ToLower().Contains(originCityName.ToLower()));
            }

            // Filter by Destination City Name (case-insensitive)
            if (!string.IsNullOrWhiteSpace(destinationCityName))
            {
                query = query.Where(t => t.Route.Destination.NameEn.ToLower().Contains(destinationCityName.ToLower()));
            }

            // Filter by Departure Date (Optional)
            if (departureDate.HasValue)
            {
                query = query.Where(t => t.DepartureDate == DateOnly.FromDateTime(departureDate.Value));
            }

            // Filter by Bus Feature (case-insensitive substring match)
            if (!string.IsNullOrWhiteSpace(busFeature))
            {
                query = query.Where(t => t.Bus.Features.ToLower().Contains(busFeature.ToLower()));
            }

            // Apply Sorting
            query = sortBy?.ToLower() switch
            {
                "departure_time" => query.OrderBy(t => t.DepartureTime),
                "available_seats" => query.OrderByDescending(t => t.AvailableSeats),
                "price_asc" => query.OrderBy(t => t.PriceJod),
                "price_desc" => query.OrderByDescending(t => t.PriceJod),
                _ => query.OrderByDescending(t => t.DepartureDate) // Default: newest first
            };

            return await query.ToListAsync();
        }
    }
}