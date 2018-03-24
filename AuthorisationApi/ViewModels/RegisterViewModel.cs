using AuthorisationApi.Models;

namespace AuthorisationApi.ViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public User ToUser()
        {
            return new User
            {
                Email = Email,
                UserName = Email,
                PhoneNumber = ""
            };
        }
    }
}