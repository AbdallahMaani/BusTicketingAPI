using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bus_ticketing_Backend.Services 
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userReposiotry;
        private readonly IConfiguration _config; 
        // what is IConfiguration? it's used to access configuration settings in .NET applications.

        public AuthService(IUserRepository repo, IConfiguration config)
        {
            _userReposiotry = repo;
            _config = config;
        }

        public async Task<User?> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userReposiotry.GetByUsernameAsync(dto.Username); 
            if (existingUser != null) // If the username is already used return null
                return null;

            var user = new User
            {
                UserId = Guid.NewGuid(), 
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "customer",
                Balance = 0
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, dto.Password);

            await _userReposiotry.AddUserAsync(user);
            return user;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userReposiotry.GetByUsernameAsync(dto.Username);
            if (user == null) return null;

            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateJwt(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private string CreateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 

            await _userReposiotry.UpdateUserAsync(user); 
            return refreshToken;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userReposiotry.GetUserByIdAsync(userId);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
        {

            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return false;

            var user = await _userReposiotry.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);

            if (result == PasswordVerificationResult.Failed)
                return false;

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            await _userReposiotry.UpdateUserAsync(user);

            return true;
        }

        public async Task<bool> ForceResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userReposiotry.GetUserByIdAsync(userId);
            if (user == null) return false;

            // No Old Password check needed for Admin

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            await _userReposiotry.UpdateUserAsync(user);

            return true;
        }

    }
}