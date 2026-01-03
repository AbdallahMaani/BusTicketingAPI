using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Services;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IAuthService _authService;

        public UserController(IUserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        // --- GET Operations ---

        // 1. Admin sees everyone
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTOs>>> GetAll()
        {
            var users = await _repository.GetAllUsersAsync();
            var result = users.Select(MapToDto);
            return Ok(result);
        }

        // 2. "Get Me" - Essential for the Frontend to show Balance/Name
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDTOs>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(MapToDto(user));
        }

        // 3. Get By ID (Secured)
        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<UserDTOs>> GetById([FromRoute] Guid id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // SECURITY: Only Admin or the User themselves can view the full profile
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
            {
                return Forbid();
            }

            return Ok(MapToDto(user));
        }

        // --- WRITE Operations ---

        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, UpdateUserDTO dto)
        {
            // SECURITY CHECK: Am I allowed to update this ID?
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            
            await _repository.UpdateUserAsync(user);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            // SECURITY CHECK
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            await _repository.DeleteUserAsync(id);
            return NoContent();
        }

        // --- WALLET Logic ---
        
        [Authorize]
        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be greater than zero.");

            var userId = GetCurrentUserId();
            var success = await _repository.AddBalanceAsync(userId, amount);

            if (!success) return BadRequest("User not found.");

            return Ok(new { message = "Balance updated successfully." });
        }

        // --- AUTH Logic (Kept mostly the same) ---

        [HttpPost("register")]
        public async Task<ActionResult<UserDTOs>> Register(RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            if (user == null) return BadRequest("Username already exists.");
            return Ok(MapToDto(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null) return BadRequest("Invalid username or password.");
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenDto request)
        {
            var result = await _authService.RefreshTokensAsync(request);
            if (result == null) return Unauthorized("Invalid token.");
            return Ok(result);
        }

        // --- Helpers ---

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        private static UserDTOs MapToDto(User u) => new UserDTOs
        {
            UserId = u.UserId,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role,
            Balance = u.Balance
        };
    }
}