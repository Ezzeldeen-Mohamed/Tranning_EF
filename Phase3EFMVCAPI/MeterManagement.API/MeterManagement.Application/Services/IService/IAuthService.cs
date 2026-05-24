using MeterManagement.Application.Common;
using MeterManagement.Application.DTOs.UserDtos;

namespace MeterManagement.Application.Services.IService
{
    /// <summary>
    /// Defines authentication and authorization operations, including user login, registration, and role management.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user based on login credentials.
        /// </summary>
        /// <param name="dto">The data transfer object containing the user's login credentials.</param>
        /// <returns>A base response containing an authentication token (e.g., JWT) if successful.</returns>
        Task<BaseResponse<string>> Login(LoginDto dto);

        /// <summary>
        /// Registers a new user within the system.
        /// </summary>
        /// <param name="dto">The data transfer object containing user registration details.</param>
        /// <returns>A base response indicating whether the registration was successful.</returns>
        Task<BaseResponse<bool>> Register(RegisterDto dto);

        /// <summary>
        /// Modifies an existing user's role assignment.
        /// </summary>
        /// <param name="dto">The data transfer object containing target user and identity details.</param>
        /// <param name="currentAdminId">The unique identifier of the administrator executing the change.</param>
        /// <returns>A base response indicating whether the role modification was successful.</returns>
        Task<BaseResponse<bool>> ChangeUserRole(ChangeRoleDto dto, string currentAdminId);

        /// <summary>
        /// Retrieves a list of all registered users in the system.
        /// </summary>
        /// <returns>A base response wrapping a list of user details.</returns>
        Task<BaseResponse<List<UserDto>>> GetAllUsers();

        /// <summary>
        /// Retrieves details of a specific user using their email address.
        /// </summary>
        /// <param name="email">The email address of the target user.</param>
        /// <returns>A base response containing the user's details.</returns>
        Task<BaseResponse<UserDto>> GetByEmail(string email);

        /// <summary>
        /// Gets all system roles explicitly assigned to a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of role names associated with the user.</returns>
        Task<IList<string>> GetRoles(string userId);
    }
}