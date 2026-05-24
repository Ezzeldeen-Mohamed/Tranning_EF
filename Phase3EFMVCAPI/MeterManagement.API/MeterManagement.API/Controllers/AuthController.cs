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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response =
                await _authService.Register(dto);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Created(
                string.Empty,
                response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response =
                await _authService.Login(dto);

            if (!response.IsSuccess)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole(
            ChangeRoleDto dto)
        {
            var adminId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized();
            }

            var response =
                await _authService.ChangeUserRole(
                    dto,
                    adminId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response =
                await _authService.GetAllUsers();

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetByEmail(
            string email)
        {
            var response =
                await _authService.GetByEmail(email);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
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
