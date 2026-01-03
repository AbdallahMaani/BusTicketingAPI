using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketing_Backend.Services;
using Bus_ticketingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")] // this line defines the base route for all endpoints in this controller.
                                // [Controller] will be replaced by the controller's name minus the "Controller" suffix, so in this case, it becomes "api/user".
    [ApiController] // This attribute indicates that the class is an API controller.
    public class UserController : ControllerBase // Inherits from ControllerBase to provide basic functionalities for handling HTTP requests.
    {
        private readonly IUserRepository _repository;
        private readonly IAuthService _authService;

        public UserController(IUserRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTOs>>> GetAll()
        {
            var users = await _repository.GetAllUsersAsync();
            var result = users.Select(MapToDto); //select : Projects each element of a list into a new form.
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDTOs>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(MapToDto(user)); //// 200 OK + user data Wrap it in an HTTP 200 OK response
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<UserDTOs>> GetById([FromRoute] Guid id)
            // ActionResult is a wrapper that lets you return different HTTP responses:
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // Only Admin or the User themselves can view the full profile
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
            {
                return Forbid();
            }
            return Ok(MapToDto(user));
        }

        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, UpdateUserDTO dto) 
            // [FromRoute] Guid id : get the user id from the route parameter not from the request body
        {
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
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            await _repository.DeleteUserAsync(id);
            return NoContent();
        }

        
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

        [HttpPost("register")]
        public async Task<ActionResult<UserDTOs>> Register(RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            if (user == null) return BadRequest("Username already exists.");
            return Ok(MapToDto(user)); 
// Breaking it down: 1.MapToDto(user) - Convert User entity to UserDTOs 2.Ok(...) - Wrap it in an HTTP 200 OK response 3.  return -Send to frontend
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

/*
 * What is ActionResult?
ActionResult is a wrapper that lets you return different HTTP responses:

 return Ok(user);              // 200 OK + user data
return NotFound();            // 404 Not Found
return BadRequest("error");   // 400 Bad Request
return Unauthorized();        // 401 Unauthorized
return Forbid();              // 403 Forbidden
return NoContent();           // 204 No Content


var result = users.Select(MapToDto); 
Select :
Projects each element of a list into a new form.
users = [User1, User2, User3]
           ↓
        Select(MapToDto)
           ↓
result = [UserDTO1, UserDTO2, UserDTO3]
*/