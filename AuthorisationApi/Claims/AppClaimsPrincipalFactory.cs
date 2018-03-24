using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorisationApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AuthorisationApi.Claims
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    public class AppClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
        where TUser : User
        where TRole : Role
    {
        private readonly IdentityOptions _options;

        /// <summary>
        ///
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="optionsAccessor"></param>
        public AppClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userId = await UserManager.GetUserIdAsync(user);
            var userName = await UserManager.GetUserNameAsync(user);

            var id = new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationScheme,
                Options.ClaimsIdentity.UserNameClaimType,
                Options.ClaimsIdentity.RoleClaimType
            );

            id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userName));

            if (UserManager.SupportsUserSecurityStamp)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType,
                    await UserManager.GetSecurityStampAsync(user)));
            }

            if (UserManager.SupportsUserRole)
            {
                var roles = await UserManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                    if (RoleManager.SupportsRoleClaims)
                    {
                        var role = await RoleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            id.AddClaims(await RoleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }

            if (UserManager.SupportsUserClaim)
            {
                id.AddClaims(await UserManager.GetClaimsAsync(user));
            }

            ClaimsPrincipal principal = new ClaimsPrincipal(id);

            if (principal.Identity is ClaimsIdentity)
            {
                ClaimsIdentity identity = (ClaimsIdentity) principal.Identity;

                Claim userIdClaim = new Claim("UserId", user.Id.ToString());

                if (!identity.HasClaim(userIdClaim.Type, userIdClaim.Value))
                {
                    identity.AddClaim(userIdClaim);
                }

                Claim emailClaim = new Claim(ClaimTypes.Email, user.Email);
                if (!identity.HasClaim(emailClaim.Type, emailClaim.Value))
                {
                    identity.AddClaim(emailClaim);
                }
            }

            return principal;
        }
    }
}