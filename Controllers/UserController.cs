using Bus_ticketing_Backend.DTOs;
using Bus_ticketing_Backend.IRepositories;
using Bus_ticketingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bus_ticketing_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository repository) => _repository = repository;

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


        [HttpPost]
        public async Task<ActionResult> Create(CreateUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password, // hash
                Phone = dto.Phone,
                Role = "customer",
                Balance = 0
            };

            await _repository.AddUserAsync(user);

            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, null); 
        }


        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> Update([FromRoute] Guid id, UpdateUserDTO dto)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;

            await _repository.UpdateUserAsync(user);
            return NoContent();
        }


        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _repository.DeleteUserAsync(id);
            return NoContent();
        }
    }
}