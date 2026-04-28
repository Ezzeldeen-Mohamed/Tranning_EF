using MeterManagement.Application.DTOs.UserDtos;

namespace MeterManagement.Application.Services.IService
{
    public interface IAuthService
    {
        Task<string> Login(LoginDto dto);
        Task<bool> Register(RegisterDto dto);
        Task<bool> ChangeUserRole(ChangeRoleDto dto, string currentAdminId);
        Task<List<UserDto>> GetAllUsers();
    }
}
