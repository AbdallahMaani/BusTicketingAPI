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
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config; 
        // what is IConfiguration? it's used to access configuration settings in .NET applications.

        public AuthService(IUserRepository repo, IConfiguration config)
        {
            _userRepository = repo;
            _config = config;
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterDto dto)
        {
            var result = new RegisterResultDto();

            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username); 
            if (existingUser != null)
            {
                result.Success = false;
                result.ErrorMessage = "Username is already taken.";
                return result;
            }

            var passwordValidation = PasswordValidator.ValidatePassword(dto.Password);
            if (!passwordValidation.IsValid)
            {
                result.Success = false;
                result.ErrorMessage = passwordValidation.ErrorMessage;
                return result;
            }

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

            await _userRepository.AddUserAsync(user);

            result.Success = true;
            result.ErrorMessage = null;
            result.User = new UserDTOs
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Balance = user.Balance
            };

            return result;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null) return null;

            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            var accessToken = CreateJwt(user);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            // Store refresh token for use in controller to set cookie
            return new TokenResponseDto
            {
                AccessToken = accessToken,
                User = new AuthUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Balance = user.Balance
                }
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
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateUserAsync(user);
            return refreshToken;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return new TokenResponseDto
            {
                AccessToken = CreateJwt(user),
                User = new AuthUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Balance = user.Balance
                }
            };
        }

        public async Task<ChangePasswordResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return new ChangePasswordResultDto { Success = false, ErrorMessage = "Old password and new password cannot be empty." };

            var passwordValidation = PasswordValidator.ValidatePassword(request.NewPassword);
            if (!passwordValidation.IsValid)
                return new ChangePasswordResultDto { Success = false, ErrorMessage = passwordValidation.ErrorMessage };

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new ChangePasswordResultDto { Success = false, ErrorMessage = "User not found." };

            var passwordHasher = new PasswordHasher<User>();
            var verifyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
                return new ChangePasswordResultDto { Success = false, ErrorMessage = "Old password is incorrect." };

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return new ChangePasswordResultDto { Success = true, ErrorMessage = null };
        }

        public async Task<AdminResetPasswordResultDto> ForceResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new AdminResetPasswordResultDto { Success = false, ErrorMessage = "User not found." };

            var passwordValidation = PasswordValidator.ValidatePassword(newPassword);
            if (!passwordValidation.IsValid)
                return new AdminResetPasswordResultDto { Success = false, ErrorMessage = passwordValidation.ErrorMessage };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            await _userRepository.UpdateUserAsync(user);

            return new AdminResetPasswordResultDto { Success = true, ErrorMessage = null };
        }
    }
}