using BaseApi.Data;
using BaseApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BaseApi.Utilities;
using System.Linq;
using System.Security.Cryptography;

namespace BaseApi.Services
{
    public interface IAuthenticationService
    {
        void AddUserTokenAsync(UserToken userToken);
        void AddUserTokenAsync(Users user, string refreshToken, string accessToken, string refreshTokenSource);
        bool IsValidTokenAsync(string accessToken, Guid userId);
        UserToken FindTokenAsync(string refreshToken);
        void DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        (string accessToken, string refreshToken, IEnumerable<Claim> Claims) CreateJwtTokens(Users user, string refreshTokenSource);
        void RevokeUserBearerTokensAsync(string userIdValue, string refreshToken);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly BaseApiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthenticationService(BaseApiDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public (string accessToken, string refreshToken, IEnumerable<Claim> Claims) CreateJwtTokens(Users users, string refreshTokenSource)
        {
            var user = (from userData in _dbContext.Users
                        where userData.Email == users.Email
                        select userData).FirstOrDefault();
            var result = createAccessTokenAsync(user, user.Email);
            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            AddUserTokenAsync(user, refreshToken, result.AccessToken, refreshTokenSource);
            return (result.AccessToken, refreshToken, result.Claims);
        }

        private (string AccessToken, IEnumerable<Claim> Claims) createAccessTokenAsync(Users user, string email)
        {


            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(),_configuration["JwtIssuer"]),
                new Claim(JwtRegisteredClaimNames.Jti, email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(),_configuration["JwtIssuer"]),
                  new Claim(ClaimTypes.Name, user.Name, ClaimValueTypes.String,_configuration["JwtIssuer"]),
                 new Claim("UserId",user.Id.ToString(),_configuration["JwtIssuer"]),
                  new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration["JwtIssuer"])
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["AccessTokenExpirationMinutes"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: DateTime.Now.AddYears(5),
                signingCredentials: creds
            ); ;

            return (new JwtSecurityTokenHandler().WriteToken(token), claims);
        }


        public void AddUserTokenAsync(Users user, string refreshToken, string accessToken, string refreshTokenSource)
        {
            var now = DateTimeOffset.UtcNow;
            var token = new UserToken
            {
                UserId = user.Id,
                // Refresh token handles should be treated as secrets and should be stored hashed
                RefreshTokenIdHash = GetSha256Hash(refreshToken),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSource) ?
                                           null : GetSha256Hash(refreshTokenSource),
                AccessTokenHash = GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = DateTime.MaxValue,
                AccessTokenExpiresDateTime = DateTime.Now.AddYears(5)
            };
            AddUserTokenAsync(token);
        }


        public void AddUserTokenAsync(UserToken userToken)
        {
            if (!Convert.ToBoolean(_configuration["AllowMultipleLoginsFromTheSameUser"]))
            {
                InvalidateUserTokensAsync(userToken.UserId);
            }
            DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);

            _dbContext.UserToken.Add(userToken);
            _dbContext.SaveChanges();
        }


        public void InvalidateUserTokensAsync(Guid userId)
        {
            var userTokens = (from uTokens in _dbContext.UserToken
                              where uTokens.UserId == userId
                              select uTokens);

            if (userTokens.Count() > 0)
            {
                _dbContext.RemoveRange(userTokens);
                _dbContext.SaveChanges();
            }
        }
        public static string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
        public bool IsValidTokenAsync(string accessToken, Guid userId)
        {
            var accessTokenHash = GetSha256Hash(accessToken);
            var userToken = (from uTokens in _dbContext.UserToken
                             where uTokens.AccessTokenHash == accessTokenHash && uTokens.UserId == userId
                             select uTokens
                             ).FirstOrDefault();

            return userToken?.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
        }


        public UserToken FindTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            //TODO FK eklenecek UserToken CondoUser objesi ile dönecek.
            var refreshTokenIdHash = GetSha256Hash(refreshToken);
            var tokenData = (from uTokens in _dbContext.UserToken
                             where uTokens.RefreshTokenIdHash == refreshTokenIdHash
                             select uTokens).FirstOrDefault();

            // _tokens.GetWhere(x => x.RefreshTokenIdHash == refreshTokenIdHash);
            tokenData.User = new Users();
            tokenData.User = (from users in _dbContext.Users
                              where users.Id == tokenData.UserId
                              select users).FirstOrDefault();
            return tokenData;
        }

        public void DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }


            var userTokens = (from uTokens in _dbContext.UserToken
                              where uTokens.RefreshTokenIdHashSource == refreshTokenIdHashSource
                              select uTokens).ToList();

            if (userTokens.Count > 0)
            {
                _dbContext.UserToken.RemoveRange(userTokens);
                _dbContext.SaveChanges();
            }
        }


        public void RevokeUserBearerTokensAsync(string userIdValue, string refreshToken)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && Guid.TryParse(userIdValue, out Guid userId))
            {
                if (Convert.ToBoolean(_configuration["AllowSignoutAllUserActiveClients"]))
                {
                    InvalidateUserTokensAsync(userId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshTokenIdHashSource = GetSha256Hash(refreshToken);
                DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
            }
            DeleteExpiredTokensAsync();
        }
        public void DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var expiredTokens = (from uTokens in _dbContext.UserToken
                                 where uTokens.RefreshTokenExpiresDateTime < now
                                 select uTokens).ToList();
            if (expiredTokens.Count > 0)
            {
                _dbContext.UserToken.RemoveRange(expiredTokens);
                _dbContext.SaveChanges();
            }
        }
    }
}
