using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IRoutesRepository
    {
        Task<Routes> GetRouteByIdAsync(Guid routeId);
        Task<IEnumerable<Routes>> GetAllRoutesAsync();

       // Task AddRouteAsync(Routes route);
       // Task UpdateRouteAsync(Routes route);
       // Task DeleteRouteAsync(int routeId);
    }
}
