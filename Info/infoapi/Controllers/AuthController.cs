using infoapi.DbData.Models;
using infoapi.Entities;
using infoapi.Interfaces;
using infoapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace infoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [Authorize]
        [HttpPost("Register User")]
        public async Task<IActionResult> Register(RegisterUser User)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Username) && (string.IsNullOrEmpty(User.Password) && User.RoleId <= 0))
                {
                    return BadRequest("Invalid input data. Ensure all fields are correctly provided.");
                }

                var user = _authService.RegisterUser(User);
                return Ok(new { Message = "User registered successfully", User = user });

            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUser User)
        {
            if (User.Username == null && User.Password == null)
            {
                return BadRequest("Invalid request data");
            }
            try
            {

                var token = await _authService.Login(User);
                return Ok(new { Token = token });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> Logout([FromBody] Guid sessionId)
        {
            if (sessionId == Guid.Empty)
            {
                return BadRequest("Invalid session ID");
            }

            try
            {
                await _authService.Logout(sessionId);
                return Ok(new { Message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword request)
        {
            try
            {
                var result = await _authService.ForgotPassword(request.Input);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword request)
        {
            try
            {
                var result = await _authService.ResetPassword(request);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
