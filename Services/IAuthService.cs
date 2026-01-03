using Bus_ticketing_Backend.DTOs;
using Bus_ticketingAPI.Entities;

namespace Bus_ticketing_Backend.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterDto request);
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenDto request);
        Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
        Task<TokenResponseDto> CreateTokenResponse(User user);
        Task<string> GenerateAndSaveRefreshTokenAsync(User user);
    }
}
