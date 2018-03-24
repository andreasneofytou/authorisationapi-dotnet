using System;
using AuthrisationApi.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthrisationApi.Models
{
    public class User : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public SexEnum Sex { get; set; }
    }
}