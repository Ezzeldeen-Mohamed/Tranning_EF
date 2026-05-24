namespace MeterManagement.Application.DTOs.UserDtos
{
    /// <summary>
    /// Data transfer object containing the necessary details to update a user's system role.
    /// </summary>
    public class ChangeRoleDto
    {
        /// <summary>
        /// Gets or sets the email address of the target user whose role is being modified.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the new security or administrative role to assign (e.g., "Admin", "Technician").
        /// </summary>
        public string NewRole { get; set; }
    }
}