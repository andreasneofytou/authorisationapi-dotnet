using Microsoft.AspNetCore.Identity;

namespace AuthorisationApi.Models
{
    public class Role : IdentityRole<string>
    {
        public Role() : base()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}