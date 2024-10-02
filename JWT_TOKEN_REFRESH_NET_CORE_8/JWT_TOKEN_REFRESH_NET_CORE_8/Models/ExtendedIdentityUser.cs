using Microsoft.AspNetCore.Identity;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Models
{
    public class ExtendedIdentityUser 
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
