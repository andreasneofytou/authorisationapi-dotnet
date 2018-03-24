namespace AuthorisationApi.Results
{
    public class LoginResult
    {
        public bool IsSuccessful { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}