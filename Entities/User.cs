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

// We save it here becuase it a random string and it has NO claims, So the only way to verify to search for it in DB while Access token has claims
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}

