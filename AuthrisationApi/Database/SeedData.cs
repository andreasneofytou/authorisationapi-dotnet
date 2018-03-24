using System;
using System.Collections.Generic;
using AuthrisationApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthrisationApi.Database
{
    public static class SeedData
    {
        private static RoleManager<Role> _roleManager;
        private static UserManager<User> _userManager;

        internal static void Initialise(IServiceProvider provider)
        {
            _roleManager = provider.GetService<RoleManager<Role>>();
            _userManager = provider.GetService<UserManager<User>>();

            CreateRoles();
            CreateUsers();
            var user = _userManager.FindByEmailAsync("andreas3115@gmail.com").Result;
            if (user == null) return;
            if (_roleManager.RoleExistsAsync("Superuser").Result && _roleManager.RoleExistsAsync("Admin").Result)
            {
                var res = _userManager.AddToRolesAsync(user, new List<string> {"Admin", "Superuser"});
            }
        }

        private static void CreateRoles()
        {
            var roles = new List<string> {"Superuser", "Admin", "AppUser"};

            foreach (var role in roles)
            {
                var result = _roleManager.RoleExistsAsync(role).Result;
                if (!result)
                {
                    Role newRole = new Role(role);
                    IdentityResult res = _roleManager.CreateAsync(newRole).Result;
                }
            }
        }

        private static void CreateUsers()
        {
            var user = new User
            {
                Email = "andreas3115@gmail.com",
                UserName = "andreas3115@gmail.com",
                EmailConfirmed = true,
                FirstName = "Andreas",
                LastName = "Neofytou"
            };
            var result = _userManager.CreateAsync(user, "indigohome67").Result;
        }
    }
}