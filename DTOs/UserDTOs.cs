using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    
        public class UserDTOs
        {
            public Guid UserId { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Role { get; set; }
            public decimal Balance { get; set; }
        }
        public class CreateUserDto
        {
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
        }
        public class UpdateUserDTO
        {
            public string FullName { get; set; }
            public string Phone { get; set; }
        }
    }

