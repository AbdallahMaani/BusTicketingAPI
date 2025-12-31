using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IStationRepository
    {
        Task<Station> GetStationByIdAsync(Guid stationId);
        Task<IEnumerable<Station>> GetAllStationsAsync();
        Task AddStationAsync(Station station);
        Task UpdateStationAsync(Station station);
        Task DeleteStationAsync(Guid stationId);
    }
}
