namespace JWT_TOKEN_REFRESH_NET_CORE_8.Models
{
    public class LoginResponse
    {
        public bool IsLogedIn { get; set; } = false;
        public string JwtToken { get; set; }
        public string RefreshToken { get; internal set;}
    }
}
