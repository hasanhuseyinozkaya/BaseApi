using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseApi.Models;
using BaseApi.Services;

namespace BaseApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        // GET: api/Authentication
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult Login([FromBody] Users  user)
        {
            var (accessToken, refreshToken, claims) = _authenticationService.CreateJwtTokens(user, refreshTokenSource: null);
            return Ok(new TokenResponseDto { access_token = accessToken, refresh_token = refreshToken });
        }

        [AllowAnonymous]
        [HttpGet("RefreshToken/{refreshToken}")]
        public ActionResult RefreshToken(string refreshToken)
        {
            var token = _authenticationService.FindTokenAsync(refreshToken);

            if (token == null)
            {
                return Unauthorized("err");
            }
            var jwtResponse = _authenticationService.CreateJwtTokens(token.User, refreshToken);
            return Ok(new TokenResponseDto { access_token = jwtResponse.accessToken, refresh_token = jwtResponse.refreshToken });
        }
        [Authorize]
        [HttpGet("Logout/{refreshToken}")]
        public ActionResult Logout(string refreshToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            _authenticationService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);
            return Ok(true);
        }

        [Authorize]
        [HttpGet("[action]"), HttpPost("[action]")]
        public ActionResult IsAuthenticated()
        {
            if (User.Identity.IsAuthenticated)
                return Ok(new SuccessResponseDto { Message = "true" });
            else
                return Unauthorized(new ErrorResponseDto { Message = "false" });

        }

    }
}
