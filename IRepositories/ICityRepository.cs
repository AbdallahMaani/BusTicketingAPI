using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface ICityRepository
    {
        Task<City> GetCityByIdAsync(string cityId);
        Task<IEnumerable<City>> GetAllCitiesAsync();

        //Task AddCityAsync(City city);
        //Task UpdateCityAsync(City city);
        //Task DeleteCityAsync(int cityId);
    }
}
