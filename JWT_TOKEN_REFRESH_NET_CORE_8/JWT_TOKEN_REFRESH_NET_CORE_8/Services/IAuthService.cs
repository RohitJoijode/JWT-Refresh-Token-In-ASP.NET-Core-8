using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using static JWT_TOKEN_REFRESH_NET_CORE_8.Controllers.AuthController;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Services
{
    public interface IAuthService
    {

        Task<bool> RegisterUser(RegisterUserModel user);

        Task<LoginResponse> Login(LogInUser user);

        Task<LoginResponse> RefreshToken(RefreshTokenModel model);

    }
}
