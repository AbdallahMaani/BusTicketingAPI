namespace Bus_ticketing_Backend.DTOs
{
    // Returned to client (NO sensitive data)
    // DTO is similiar to Fire wall, it prevents sensitive data from being exposed to the client.
    //DTOs belonge to the Controller (API Layer) they contrect for what the frontend see
    //Repository belongs to the Data Layer and entities
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

    public class RegisterDto
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
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDto
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
    }

    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class AdminResetPasswordDto
    {
        public string NewPassword { get; set; }
    }
}
