using Bus_ticketing_Backend.DTOs;

namespace Bus_ticketing_Backend.Services
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterDto request);
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken);
        Task<ChangePasswordResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
        Task<AdminResetPasswordResultDto> ForceResetPasswordAsync(Guid userId, string newPassword);
    }
}
