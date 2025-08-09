using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Models;
using TaskManagement.Application.Models.Users;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IUserProfileService userProfile, IConfiguration configuration)
        {
            _userService = userService;
            _userProfileService = userProfile;
            _configuration = configuration;
        }

        [HttpPost("register")]

        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModel model)
        {
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserModel model)
        {
            var result = await _userService.LoginAsync(model);
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] OtpVerificationModel model)
        {
            var result = await _userService.VerifyOtpAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-user-auth")]
        public async Task<IActionResult> GetUserAuth()
        {
            var result = await _userService.GetUserAuth();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            
            Console.WriteLine("TestAuth endpoint called - User authenticated!");
            Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Claims count: {User.Claims.Count()}");
            
            return Ok(new
            {
                Message = "Authentication successful",
                UserId = userId,
                UserName = userName,
                Email = email,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value })
            });
        }

        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            
            return Ok(new
            {
                Message = "Debug endpoint (no auth required)",
                HasAuthHeader = !string.IsNullOrEmpty(authHeader),
                AuthHeader = authHeader?.Substring(0, Math.Min(50, authHeader?.Length ?? 0)) + "...",
                IsAuthenticated = User.Identity?.IsAuthenticated,
                UserIdentityName = User.Identity?.Name,
                ClaimsCount = User.Claims.Count(),
                Claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList()
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Application.Models.Users.ForgotPasswordRequest request)
        {
            var result = await _userService.ForgotPasswordAsync(request.Email);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Application.Models.Users.ResetPasswordRequest request)
        {
            var result = await _userService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
            return Ok(result);
        }

        [HttpDelete("by-email")]
        public async Task<IActionResult> DeleteUserByEmail([FromQuery] string email)
        {
            var result = await _userService.DeleteUserAsync(email);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpPost("resend-code/{email}")]
        public async Task<IActionResult> ResendCode([FromRoute] string email)
        {
            var result = await _userService.ResendOtpAsync(email);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
