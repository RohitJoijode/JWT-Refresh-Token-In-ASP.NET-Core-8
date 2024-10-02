using Azure.Identity;
using JWT_TOKEN_REFRESH_NET_CORE_8.DAL;
using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static JWT_TOKEN_REFRESH_NET_CORE_8.Controllers.AuthController;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly DBEngine _DbEngine;

        public AuthServices(IConfiguration config,DBEngine DbEngine)
        {
            _config = config;
            _DbEngine = DbEngine;
        }


        public async Task<bool> RegisterUser(RegisterUserModel user)
        {
            DAL.Tbl_Users1 Tbl_Users = null;

            using (var transaction = _DbEngine.Database.BeginTransaction())
            {
                var UserExits = _DbEngine.Tbl_Users1.Where(x => x.UserName == user.UserName
                                                               && x.UserEmail == user.UserEmail
                                                               && x.UserMobile == user.UserMobile
                                                               && x.IsActive == true
                                                             ).FirstOrDefault();


                if (UserExits == null)
                {
                    //using (var transaction = _DbEngine.Database.BeginTransaction())
                    //{

                    Tbl_Users = new DAL.Tbl_Users1();
                    var maxUserId = await _DbEngine.Tbl_Users1.MaxAsync(u => (int?)u.UserId) ?? 0;
                    Tbl_Users.UserId = maxUserId + 1;
                    Tbl_Users.UserName = user.UserName;
                    Tbl_Users.UserPassword = user.UserPassword;
                    Tbl_Users.UserEmail = user.UserEmail;
                    Tbl_Users.UserRole = user.UserRole;
                    Tbl_Users.UserMobile = user.UserMobile;
                    Tbl_Users.IsActive = true;
                    Tbl_Users.CreatedOn = DateTime.Now;
                    _DbEngine.Tbl_Users1.Add(Tbl_Users);
                    _DbEngine.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    if (UserExits != null)
                    {
                        Tbl_Users = new DAL.Tbl_Users1();
                        Tbl_Users = UserExits;
                        Tbl_Users.ModifyOn = DateTime.Now;
                        Tbl_Users.IsActive = true;
                        _DbEngine.Entry(Tbl_Users).State = EntityState.Modified;
                    }
                    _DbEngine.SaveChanges();
                    transaction.Commit();
                }
            }
            
            return true;
        }

        public async Task<LoginResponse> Login(LogInUser user)
        {
            DAL.Tbl_RefreshToken1 Tbl_RefreshToken = null;
            var response = new LoginResponse();
            
            var UserExits = _DbEngine.Tbl_Users1.Where(x => x.UserName == user.UserName
                                                               && x.UserPassword == user.Password
                                                             ).FirstOrDefault();

            if (UserExits is null || !(await _DbEngine.Tbl_Users1
                          .AnyAsync(x => x.UserName == user.UserName && x.UserPassword == user.Password)))
            {
                return response;
            }

            response.IsLogedIn = true;
            response.JwtToken = this.GenerateTokenString(UserExits);
            response.RefreshToken = this.GenerateRefreshTokenString();

            using (var transaction = _DbEngine.Database.BeginTransaction())
            {
                var FoundRefreshToken = _DbEngine.Tbl_RefreshToken1.Where(
                                                                           x => Convert.ToInt64(x.UserId) == UserExits.UserId
                                                                           ).FirstOrDefault();

                if (FoundRefreshToken == null)
                {
                    Tbl_RefreshToken = new DAL.Tbl_RefreshToken1();
                    var maxRefreshTokenId = await _DbEngine.Tbl_RefreshToken1.MaxAsync(u => (int?)u.Id) ?? 0;
                    Tbl_RefreshToken.Id = maxRefreshTokenId + 1;
                    Tbl_RefreshToken.Token = response.JwtToken;
                    Tbl_RefreshToken.UserId = UserExits.UserId.ToString();
                    Tbl_RefreshToken.RefreshToken = response.RefreshToken;
                    Tbl_RefreshToken.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                    Tbl_RefreshToken.IsActive = true;
                    _DbEngine.Tbl_RefreshToken1.Add(Tbl_RefreshToken);
                    _DbEngine.SaveChanges();
                    transaction.Commit();
                } else if (FoundRefreshToken != null)
                {
                        Tbl_RefreshToken = new DAL.Tbl_RefreshToken1();
                        Tbl_RefreshToken = FoundRefreshToken;
                        Tbl_RefreshToken.Token = response.JwtToken;
                        Tbl_RefreshToken.RefreshToken = response.RefreshToken;
                        Tbl_RefreshToken.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                        Tbl_RefreshToken.ModifyOn = DateTime.Now;
                        Tbl_RefreshToken.IsActive = true;
                        _DbEngine.Entry(Tbl_RefreshToken).State = EntityState.Modified;
                        _DbEngine.SaveChanges();
                         transaction.Commit();
                }
            }
            //identityUser.RefreshToken = response.RefreshToken;
            //identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);
            //await _userManager.UpdateAsync(identityUser);

            return response;
        }

        

        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateTokenString(Tbl_Users1 UsersObj)
        {
            var claims = new[]
            {
                new Claim("UserId",Convert.ToString(UsersObj.UserId)),
                new Claim("UserName",UsersObj.UserName ?? ""),
                new Claim("UserRole",UsersObj.UserRole ?? ""),
                new Claim("UserEmail",UsersObj.UserEmail ?? ""),
                new Claim("UserMobile",UsersObj.UserMobile ?? ""),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("082F41538C1178DE768A9AC86291678D"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse("2")),
                signingCredentials: creds
          );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenModel model)
        {
            Tbl_Users1 Tbl_Users1Obj = new Tbl_Users1();
            DAL.Tbl_RefreshToken1 Tbl_RefreshToken = null;
            string userId = "", userName = "", UserRole = "", UserEmail = "", UserMobile = "";
            var principal = GetTokenPrincipal(model.JwtToken);
            //principal.Identity.Name
            var response = new LoginResponse();
            if (principal?.Identity?.IsAuthenticated == false)
                return response;


            if (principal?.Identity is ClaimsIdentity claimsIdentity)
            {
                // Now you can access the claims
                var UserIdClaim = claimsIdentity.FindFirst("UserId");
                var UserNameClaim = claimsIdentity.FindFirst("UserName");
                var UserRoleClaim = claimsIdentity.FindFirst("UserRole");
                var UserEmailClaim = claimsIdentity.FindFirst("UserEmail");
                var UserMobileClaim = claimsIdentity.FindFirst("UserMobile");

                // Get the value of a specific claim
                userId = UserIdClaim?.Value;
                userName = UserNameClaim?.Value;
                UserRole = UserRoleClaim?.Value;
                UserEmail = UserEmailClaim?.Value;
                UserMobile = UserMobileClaim?.Value;


                Tbl_Users1Obj.UserId = Convert.ToInt32(userId);
                Tbl_Users1Obj.UserName = userName;
                Tbl_Users1Obj.UserRole = UserRole;
                Tbl_Users1Obj.UserEmail = UserEmail;
                Tbl_Users1Obj.UserMobile = UserMobile;
                // Or iterate through all claims
                //foreach (var claim in claimsIdentity.Claims)
                //{
                //    string claimType = claim.Type;
                //    string claimValue = claim.Value;
                //}
            }

            var FoundRefreshToken = _DbEngine.Tbl_RefreshToken1.Where(
                                                                          x => x.UserId == userId
                                                                          ).FirstOrDefault();

            if (principal?.Identity?.IsAuthenticated == false || FoundRefreshToken?.RefreshToken != model.RefreshToken || FoundRefreshToken?.RefreshTokenExpiry < DateTime.Now)
                return response;
            //if (identityUser is null || identityUser.RefreshToken != model.RefreshToken || identityUser.RefreshTokenExpiry > DateTime.Now)
            //return response;

            response.IsLogedIn = true;
            response.JwtToken = this.GenerateTokenString(Tbl_Users1Obj); //temporyt commented
            response.RefreshToken = this.GenerateRefreshTokenString();

            using (var transaction = _DbEngine.Database.BeginTransaction())
            {
                if (FoundRefreshToken == null)
                {
                    Tbl_RefreshToken = new DAL.Tbl_RefreshToken1();
                    var maxRefreshTokenId = await _DbEngine.Tbl_RefreshToken1.MaxAsync(u => (int?)u.Id) ?? 0;
                    Tbl_RefreshToken.Id = maxRefreshTokenId + 1;
                    Tbl_RefreshToken.Token = response.JwtToken;
                    Tbl_RefreshToken.UserId = userId;
                    Tbl_RefreshToken.RefreshToken = response.RefreshToken;
                    Tbl_RefreshToken.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                    Tbl_RefreshToken.IsActive = true;
                    _DbEngine.Tbl_RefreshToken1.Add(Tbl_RefreshToken);
                    _DbEngine.SaveChanges();
                    transaction.Commit();
                }
                else if (FoundRefreshToken != null)
                {
                    Tbl_RefreshToken = new DAL.Tbl_RefreshToken1();
                    Tbl_RefreshToken = FoundRefreshToken;
                    Tbl_RefreshToken.Token = response.JwtToken;
                    Tbl_RefreshToken.RefreshToken = response.RefreshToken;
                    Tbl_RefreshToken.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                    Tbl_RefreshToken.ModifyOn = DateTime.Now;
                    Tbl_RefreshToken.IsActive = true;
                    _DbEngine.Entry(Tbl_RefreshToken).State = EntityState.Modified;
                    _DbEngine.SaveChanges();
                    transaction.Commit();
                }
            }

            //identityUser.RefreshToken = response.RefreshToken;
            //identityUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);

            //await _userManager.UpdateAsync(identityUser);
            return response;
        }

        private ClaimsPrincipal? GetTokenPrincipal(string token)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("082F41538C1178DE768A9AC86291678D"));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securitykey,
                ValidateLifetime = false,
                ValidateActor = false,
                ValidateIssuer = false,
                ValidateAudience = false,
            };
            return new JwtSecurityTokenHandler().ValidateToken(token,validation,out _);
        }
    }
}
