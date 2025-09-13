using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CommunityForum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            _logger.LogInformation("Received POST request to register new user");
            var user = await _userService.RegisterUserAsync(request);
            return Ok(user);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            _logger.LogInformation("Received POST request to login");
            var token = await _userService.LoginUserAsync(request);
            if (token == null)
                return Forbid();
            return Ok(token);
        }
        // GET: api/<UsersController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Received GET request to get all users");
            return Ok(await _userService.GetAllUsersAsync());
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation($"Received GET request to get user by id {id}");
            return Ok(await _userService.GetUserById(id));
        }
        // PUT api/<UsersController>/
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateUserRequest request)
        {
            _logger.LogInformation($"Received PUT request to update user by id {request.Id}");
            return Ok(await _userService.UpdateUserAsync(request));
        }

        // DELETE api/<UsersController>/
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteUserRequest request)
        {
            _logger.LogInformation($"Received DELETE request to delete user by id {request.Id}");
            await _userService.DeleteUserAsync(request);
            return Ok();
        }
    }
}
