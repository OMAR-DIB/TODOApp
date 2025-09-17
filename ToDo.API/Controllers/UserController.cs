using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.Dtos.UserDtos;
using ToDo.API.Dtos.UserDtos.Login;
using ToDo.API.Services.UserServices;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDto dto)
        {
            try
            {
                var created = await _userServices.CreateUser(dto, profileImagePath: null);
                // return 201 with id and message
                return CreatedAtAction(nameof(Register), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto dto)
        {
            var res = await _userServices.VerifyEmailAsync(dto);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequestDto dto)
        {
            var res = await _userServices.ResendVerificationAsync(dto);
            if (res.Sent) return Ok(res);
            return BadRequest(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var res = await _userServices.LoginAsync(dto);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials or email not confirmed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return StatusCode(500, new { message = "An error occurred." });
            }
        }
    }
}
