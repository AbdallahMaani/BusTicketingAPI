using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> AddBalanceAsync(Guid userId, decimal amount);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task InvalidateRefreshTokenAsync(string refreshToken);
        Task DeleteUserAsync(Guid userId);
    }
}
