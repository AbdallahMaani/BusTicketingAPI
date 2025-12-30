using System.ComponentModel.DataAnnotations;

namespace Bus_ticketingAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // In production, hash this!
        public string Phone { get; set; }
        public string Role { get; set; } // e.g., "customer" or "admin"

        public decimal Balance { get; set; }
    }
}