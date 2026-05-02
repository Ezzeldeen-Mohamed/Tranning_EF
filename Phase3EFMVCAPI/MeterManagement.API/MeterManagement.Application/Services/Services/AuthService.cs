using MeterManagement.API.Models;
using MeterManagement.Application.DTOs.UserDtos;
using MeterManagement.Application.Exceptions;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeterManagement.Application.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly ILocalizationService _localization;

        public AuthService(UserManager<User> userManager, IConfiguration config,
                RoleManager<IdentityRole> roleManager,
                ILogger<AuthService> logger,
                ILocalizationService localization,
                IMemoryCache cache)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _logger = logger;
            _localization = localization;
        }

        public async Task<bool> Register(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new BusinessException(_localization.GetString("RegistrationFailed"));
            }

            _logger.LogInformation("User with email {Email} registered successfully", dto.Email);
            await _userManager.AddToRoleAsync(user, Roles.Agent);
            return true;

        }

        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {

                _logger.LogWarning("Login attempt failed: User with email {Email} not found", dto.Email);
                throw new BusinessException(_localization.GetString("InvalidEmailOrPassword"));
            }

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isValid)
            {
                _logger.LogWarning("Login attempt failed: Invalid password for user with email {Email}", dto.Email);
                throw new BusinessException(_localization.GetString("InvalidEmailOrPassword"));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyString = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(keyString))
            {
                _logger.LogError("JWT Key is missing in configuration");
                throw new BusinessException(_localization.GetString("JwtKeyMissing"));
            }



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var duration = int.Parse(_config["Jwt:DurationInMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            _logger.LogInformation("User with email {Email} logged in successfully", dto.Email);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> ChangeUserRole(ChangeRoleDto dto, string currentAdminId)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Change role attempt failed: User with email {Email} not found", dto.Email);
                return false;
            }

            if (user.Id == currentAdminId)
            {
                _logger.LogWarning("Change role attempt failed: Admin with ID {AdminId} attempted to change their own role", currentAdminId);
                throw new BusinessException("You cannot change your own role");
            }
            var roleExists = await _roleManager.RoleExistsAsync(dto.NewRole);
            if (!roleExists)
            {
                _logger.LogWarning("Change role attempt failed: Role {Role} not found", dto.NewRole);
                throw new BusinessException("Role not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(dto.NewRole))
            {
                _logger.LogInformation("Change role attempt: User with email {Email} already has role {Role}", dto.Email, dto.NewRole);
                return true;
            }

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.NewRole);

            return true;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = roles.FirstOrDefault().ToString() ?? "No Role"
                });
            }
            _logger.LogInformation("Retrieved all users successfully. Total users: {UserCount}", result.Count);
            return result;
        }

    }
}
