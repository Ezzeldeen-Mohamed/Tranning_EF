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
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
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

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _authService.GetByEmail(email);
            return Ok(user);
        }


    }
}



/*

[HttpPost("refreshToken")]
public async Task<IActionResult> refreshToken()
{
    var refreshToken = Request.Cookies["refreshToken"];
    var refreshTokenResult = await _jwtService.refreshToken(refreshToken);

    setRefreshTokenInCookie(new RefreshTokenModel
    {
        Token = refreshTokenResult.refreshToken,
        Expiration = refreshTokenResult.refreshTokenExpiration
    });

    return Ok(refreshTokenResult);
}


        private void setRefreshTokenInCookie(RefreshTokenModel refreshToken)
{
    if (refreshToken != null)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expiration.ToLocalTime(),
        };
        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

*/
