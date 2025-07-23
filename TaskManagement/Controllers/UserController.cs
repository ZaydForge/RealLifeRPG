using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Attributes;
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

        public UserController(IUserService userService)
        {
            _userService = userService;
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
