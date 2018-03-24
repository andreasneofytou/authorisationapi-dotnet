using System.Collections.Generic;
using System.Linq;
using AuthrisationApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthrisationApi.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public List<User> GetUsers()
        {
            return _userManager.Users.Select(user => new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Dob = user.Dob,
                PhoneNumber = user.PhoneNumber
            }).ToList();
        }
    }
}