using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Models;
using Bus_ticketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Bus_ticketing_Backend.Repositories
{ // this repository handles all database operations related to User entity
    public class UserRepository : IUserRepository // Implementing the IUserRepository interface
    {
        private readonly AppDbContext _context; // Dependency injection of the AppDbContext _context is an instance of the Db context
        public UserRepository(AppDbContext context) => _context = context; // Constructor to initialize the context

        public async Task<User> GetUserByIdAsync(Guid userId) => 
            await _context.Users.FindAsync(userId); // FindAsync is used for primary key lookups it's a predefined method in EF Core

        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _context.Users.ToListAsync(); // ToListAsync is a predefined method in EF Core to asynchronously get all records as a list

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user); // AddAsync is a predefined method in EF Core to asynchronously add a new entity
            await _context.SaveChangesAsync(); // SaveChangesAsync is a predefined method in EF Core to asynchronously save changes to the database
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user); // Update is a predefined method in EF Core to update an existing entity
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user); // Remove is a predefined method in EF Core to delete an entity
                await _context.SaveChangesAsync();
            }
        }
    }
}