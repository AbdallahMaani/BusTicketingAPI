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
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<User?> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _repo.GetByUsernameAsync(dto.Username); 
            if (existingUser != null)
                return null;

            var user = new User
            {
                UserId = Guid.NewGuid(), // Ensure ID is generated
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "customer",
                Balance = 0
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, dto.Password);

            await _repo.AddUserAsync(user);
            return user;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByUsernameAsync(dto.Username);
            if (user == null) return null;

            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return await CreateTokenResponse(user);
        }

        // 3. Refresh Token Logic (NEW)
        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null)
                return null;

            return await CreateTokenResponse(user);
        }

        // Helper: Validate Refresh Token
        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _repo.GetUserByIdAsync(userId);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        // Helper: Create Access + Refresh Tokens
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateJwt(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        // Helper: Generate JWT
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

        // Helper: Generate & Save Refresh Token
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 

            await _repo.UpdateUserAsync(user); // Save to DB via Repo
            return refreshToken;
        }
    }
}