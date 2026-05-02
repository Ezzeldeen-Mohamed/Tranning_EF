using System.IdentityModel.Tokens.Jwt;

namespace MeterViewMangement.Helpers
{
    public class JWTbroker
    {
        public static string GetRole(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var role = jwt.Claims.FirstOrDefault(c =>
                c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            );

            return role?.Value;
        }

    }
}
