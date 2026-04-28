using MeterManagement.API.Models;
using MeterManagement.Application.DTOs.UserDtos;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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

        public AuthService(UserManager<User> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
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
                return false;

            //if (!string.IsNullOrEmpty(dto.Role))
            //{
            //    if (dto.Role != "Agent")
            //        throw new Exception("Invalid role");
            //    //await _userManager.AddToRoleAsync(user, dto.Role);
            //}

            await _userManager.AddToRoleAsync(user, Roles.Agent);
            return true;
        }

        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return null;

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isValid)
                return null;

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
                throw new Exception("JWT Key is missing in configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var duration = int.Parse(_config["Jwt:DurationInMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> ChangeUserRole(ChangeRoleDto dto, string currentAdminId)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            if (user.Id == currentAdminId)
                throw new Exception("You cannot change your own role");

            var roleExists = await _roleManager.RoleExistsAsync(dto.NewRole);
            if (!roleExists)
                throw new Exception("Role not found");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(dto.NewRole))
                return true;

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

            return result;
        }

    }
}
