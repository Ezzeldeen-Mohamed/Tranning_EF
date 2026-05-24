using MeterManagement.Application.Common;
using MeterManagement.Application.DTOs.UserDtos;
using MeterManagement.Application.Exceptions;
using MeterManagement.Application.Resources;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using MeterManagement.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeterManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly ILocalizationService _localization;

        public AuthService(
            UserManager<User> userManager,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthService> logger,
            ILocalizationService localization)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _logger = logger;
            _localization = localization;
        }

        public async Task<BaseResponse<bool>> Register(RegisterDto dto)
        {
            var existingUser =
                await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BaseResponse<bool>.Failure(
                    _localization.GetString("UserAlreadyExists"));
            }

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result =
                await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Registration failed for {Email}",
                    dto.Email);

                return BaseResponse<bool>.Failure(
                    _localization.GetString("RegistrationFailed"),
                    result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, Roles.Agent);

            _logger.LogInformation(
                "User {Email} registered successfully",
                dto.Email);

            return BaseResponse<bool>.Success(
                true,
                _localization.GetString("RegistrationSuccess"));
        }

        public async Task<BaseResponse<string>> Login(LoginDto dto)
        {
            var user =
                await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return BaseResponse<string>.Failure(
                    _localization.GetString("InvalidEmailOrPassword"));
            }

            var isValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    dto.Password);

            if (!isValid)
            {
                return BaseResponse<string>.Failure(
                    _localization.GetString("InvalidEmailOrPassword"));
            }

            var roles =
                await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(
                roles.Select(role =>
                    new Claim(ClaimTypes.Role, role)));

            var keyString = _config["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyString))
            {
                _logger.LogError("JWT key is missing");

                return BaseResponse<string>.Failure(
                    "JWT configuration error");
            }

            if (!int.TryParse(
                _config["Jwt:DurationInMinutes"],
                out int duration))
            {
                return BaseResponse<string>.Failure(
                    "Invalid JWT duration configuration");
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(keyString));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials:
                    new SigningCredentials(
                        key,
                        SecurityAlgorithms.HmacSha256));

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            _logger.LogInformation(
                "User {Email} logged in successfully",
                dto.Email);

            return BaseResponse<string>.Success(
                tokenString,
                "Login successful");
        }

        public async Task<BaseResponse<bool>> ChangeUserRole(
            ChangeRoleDto dto,
            string currentAdminId)
        {
            var user =
                await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return BaseResponse<bool>.Failure(
                    "User not found");
            }

            if (user.Id == currentAdminId)
            {
                return BaseResponse<bool>.Failure(
                    "You cannot change your own role");
            }

            var roleExists =
                await _roleManager.RoleExistsAsync(dto.NewRole);

            if (!roleExists)
            {
                return BaseResponse<bool>.Failure(
                    "Role not found");
            }

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(
                user,
                currentRoles);

            await _userManager.AddToRoleAsync(
                user,
                dto.NewRole);

            return BaseResponse<bool>.Success(
                true,
                "Role updated successfully");
        }
        public async Task<IList<string>> GetRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning(
                    "User not found while getting roles. UserId: {UserId}",
                    userId);

                throw new BusinessException(
                    _localization.GetString("UserNotFound"),
                    404);
            }

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation(
                "Roles retrieved successfully for user: {UserId}",
                user.Id);

            return roles;
        }

        public async Task<BaseResponse<List<UserDto>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles =
                    await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return BaseResponse<List<UserDto>>.Success(result);
        }

        public async Task<BaseResponse<UserDto>> GetByEmail(
            string email)
        {
            var user =
                await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BaseResponse<UserDto>.Failure(
                    _localization.GetString("UserNotFound"));
            }

            var roles =
                await _userManager.GetRolesAsync(user);

            var dto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            return BaseResponse<UserDto>.Success(dto);
        }
    }
}