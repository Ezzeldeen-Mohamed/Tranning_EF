using MeterManagement.Application.DTOs.UserDtos;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeterManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authservice)
        {
            _authService = authservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.Register(dto);

            if (!result)
                return BadRequest("Registration failed");

            return Ok("User created successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);

            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new
            {
                token = token
            });
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("ChangeRole")]
        public async Task<IActionResult> ChangeRole(ChangeRoleDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _authService.ChangeUserRole(dto, adminId);

            if (!result)
                return BadRequest("Failed");

            return Ok("Role Updated");
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            return Ok(users);
        }

    }
}
