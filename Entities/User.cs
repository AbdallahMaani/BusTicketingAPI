using System.ComponentModel.DataAnnotations;

namespace Bus_ticketingAPI.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "customer";
        public decimal Balance { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
