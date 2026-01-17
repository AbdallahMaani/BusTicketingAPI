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
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UserController(IUserRepository repository, IAuthService authService)
        {
            _userRepository = repository;
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTOs>>> GetAll()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var result = users.Select(MapToDto); //select : Projects each element of a list into a new form.
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDTOs>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(MapToDto(user)); //// 200 OK + user data Wrap it in an HTTP 200 OK response
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<UserDTOs>> GetById([FromRoute] Guid id) //Task means the method is asynchronous and will return a result in the future.
                                                                               // ActionResult is a wrapper that lets you return different HTTP responses:
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

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

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.Email = dto.Email;
            user.Role = dto.Role;

            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var currentUserId = GetCurrentUserId();
            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }

        
        [Authorize]
        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be greater than zero.");

            var userId = GetCurrentUserId();

            var success = await _userRepository.AddBalanceAsync(userId, amount);

            if (!success) return BadRequest("User not found.");

            return Ok(new { message = "Balance updated successfully." });
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResultDto>> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
        {
            var tokenResult = await _authService.LoginAsync(dto);
            if (tokenResult == null)
                return BadRequest("Invalid username or password.");

            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                return Unauthorized();

            if (user.RefreshToken != null)
            {
                SetRefreshTokenCookie(user.RefreshToken);
            }

            return Ok(tokenResult);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token not found.");

            var result = await _authService.RefreshTokensAsync(refreshToken);
            if (result == null)
                return Unauthorized("Invalid or expired refresh token.");

            return Ok(result);
        }

        // Helper method to set HTTP-only cookie
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(new { message = "Password changed successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:Guid}/reset-password")]
        public async Task<ActionResult> AdminResetPassword([FromRoute] Guid id, [FromBody] AdminResetPasswordDto dto)
        {
            var result = await _authService.ForceResetPasswordAsync(id, dto.NewPassword);

            if (!result.Success)
                return BadRequest(result);

            return Ok(new { message = "Password has been reset by Admin." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _userRepository.InvalidateRefreshTokenAsync(refreshToken);
            }

            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully." });
        }
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

    [Route("api/[controller]")] this line defines the base route for all endpoints in this controller.
                                 [Controller] will be replaced by the controller's name ,so in this case, it becomes "api/user".
    [ApiController]  This attribute indicates that the class is an API controller.
    public class UserController : ControllerBase  Inherits from ControllerBase to provide basic functionalities for handling HTTP requests.

*/