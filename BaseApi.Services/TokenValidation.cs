using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Services
{
    public interface ITokenValidation
    {
        Task ValidateAsync(TokenValidatedContext context);
    }
    public class TokenValidation : ITokenValidation
    {
        private readonly IAuthenticationService _authenticationService;

        public TokenValidation(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst("UserId").Value;
            Guid userIdFromClaims = Guid.Parse(userIdString);
            if (!Guid.TryParse(userIdFromClaims.ToString(), out Guid userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !_authenticationService.IsValidTokenAsync(accessToken.RawData, userId))
            {
                context.Fail("This token is not in our database.");
                return;
            }

        }
    }
}
