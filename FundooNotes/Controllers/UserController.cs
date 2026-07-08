using FundooNotes.BusinessLogicLayer.Interfaces;
using FundooNotes.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // Step 1 - Inject Service
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Step 2 - Write Endpoints Register Controller
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            // Call Service
            var result = await _userService.RegisterUser(dto);

            // Return Response
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result);
        }

        // Login Controller
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO dto)
        {
            // Call Service
            var result = await _userService.LoginUser(dto);

            // Return Response
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            var result = await _userService.ForgotPassword(dto.Email);

            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _userService.ResetPassword(dto);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
