namespace MeterManagement.Application.DTOs.UserDtos
{
    /// <summary>
    /// Data transfer object holding the information required to register a new user account.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Gets or sets the full legal or display name of the user.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the unique email address to link to the new account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password chosen by the user to secure their account.
        /// </summary>
        public string Password { get; set; }
    }
}