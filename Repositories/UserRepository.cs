using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Data;
using Bus_ticketingAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        // Use AsNoTracking for read-only lists to improve speed
        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.AsNoTracking().ToListAsync();

        public async Task<User?> GetUserByIdAsync(Guid userId) =>
            await _context.Users.FindAsync(userId);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // NEW: Safe method to specifically handle money
        public async Task<bool> AddBalanceAsync(Guid userId, decimal amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Balance += amount;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}