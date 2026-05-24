namespace MeterManagement.Application.DTOs.UserDtos
{
    /// <summary>
    /// Data transfer object containing authentication tokens and expiration details upon a successful authentication request.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Gets or sets the short-lived JSON Web Token (JWT) used to authorize subsequent API requests.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the long-lived refresh token used to request a new access token without re-authenticating.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the exact date and time when the refresh token becomes invalid.
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }
    }
}