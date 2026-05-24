namespace MeterManagement.Application.DTOs.UserDtos
{
    /// <summary>
    /// Data transfer object used for read operations and displaying profile information about a user.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique internal identifier for the user record.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the primary email address linked to the user account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the current primary system role assigned to the user.
        /// </summary>
        public string Role { get; set; }
    }
}