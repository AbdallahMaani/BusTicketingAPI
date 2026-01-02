using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly AuthService _authService; // Inject Service

        // Fix Constructor: Inject BOTH Repo and Service
        public UserController(IUserRepository repository, AuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        // ... GetAll, GetById, Update, Delete remain the same ...

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTOs>>> GetAll()
        {
            var users = await _repository.GetAllUsersAsync();

            var result = users.Select(u => new UserDTOs
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                Balance = u.Balance
            });

            return Ok(result);
        }

        // GET: api/User/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<UserDTOs>> GetById([FromRoute] Guid id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new UserDTOs
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Balance = user.Balance
            });
        }

        // PUT: api/User/{id}
        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, UpdateUserDTO dto)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // Update allowed fields
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            // Note: We usually don't update Passwords or Usernames here directly for security

            await _repository.UpdateUserAsync(user);
            return NoContent();
        }

        // DELETE: api/User/{id}
        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteUserAsync(id);
            return NoContent();
        }

        // 1. REGISTER
        [HttpPost("register")]
        public async Task<ActionResult<UserDTOs>> Register(RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            if (user == null)
                return BadRequest("Username already exists.");

            // Return safe DTO, not the Entity
            var userDto = new UserDTOs
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);
        }

        // 2. LOGIN
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        // 3. REFRESH TOKEN (New Endpoint)
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenDto request)
        {
            var result = await _authService.RefreshTokensAsync(request);
            if (result == null)
                return Unauthorized("Invalid or expired refresh token.");

            return Ok(result);
        }

        // 4. AUTH TEST (Optional)

        [Authorize]
        [HttpGet("authenticated-only")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
                return Ok("You are authenticated!");

            return Forbid("You are not Authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            if(User.IsInRole("Admin"))
            return Ok("You are an admin!");

            return Forbid("You are not Authorized");
        }
    }
}