namespace MeterManagement.Application.DTOs.UserDtos
{
    /// <summary>
    /// Data transfer object representing the user credentials required for system authentication.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the registered email address used as the account identifier.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the plain-text password for identity verification.
        /// </summary>
        public string Password { get; set; }
    }
}