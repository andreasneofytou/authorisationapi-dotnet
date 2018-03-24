using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthrisationApi.Models;
using AuthrisationApi.Options;
using Microsoft.AspNetCore.Identity;

namespace AuthrisationApi.TokenProviders
{
    public class JwtProvider : ITokenProvider
    {
        private readonly TokenProviderOptions _options;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public JwtProvider(TokenProviderOptions options, UserManager<User> userMananger, RoleManager<Role> roleManager)
        {
            _options = options;
            _userManager = userMananger;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateToken(User user)
        {
            var now = DateTime.UtcNow;
            var nowOffset = DateTimeOffset.UtcNow;
            IdentityOptions options = new IdentityOptions();
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, nowOffset.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id),
                new Claim(options.ClaimsIdentity.UserNameClaimType , user.Email)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}