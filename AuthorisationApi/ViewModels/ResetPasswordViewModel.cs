namespace AuthorisationApi.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }
        public string ResetToken { get; set; }
        public string Password { get; set; }
    }
}