using Bus_ticketingAPI.Models;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IBusRepository
    {
        Task<Bus> GetBusByIdAsync(Guid busId);
        Task<IEnumerable<Bus>> GetAllBusesAsync();
        Task AddBusAsync(Bus bus);
        Task UpdateBusAsync(Bus bus);
        Task DeleteBusAsync(Guid busId);
    }
}
