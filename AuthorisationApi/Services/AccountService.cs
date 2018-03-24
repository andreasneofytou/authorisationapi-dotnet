using System;
using System.Text;
using System.Threading.Tasks;
using AuthorisationApi.Models;
using AuthorisationApi.Results;
using AuthorisationApi.TokenProviders;
using AuthorisationApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AuthorisationApi.Services
{
    public class AccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly ITokenProvider _tokenProvider;
        private readonly RoleManager<Role> _roleManager;

        public AccountService(UserManager<User> userManager, EmailService emailService, ITokenProvider tokenProvider,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _tokenProvider = tokenProvider;
            _roleManager = roleManager;
        }

        public async Task<bool> Register(RegisterViewModel registerViewModel, UrlHelper urlHelper)
        {
            IdentityResult result =
                await _userManager.CreateAsync(registerViewModel.ToUser(), registerViewModel.Password);

            if (result.Succeeded)
            {
                User user = await _userManager.FindByEmailAsync(registerViewModel.Email);
                await _userManager.AddToRoleAsync(user, "AppUser");
                string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string confirmUrl = urlHelper.Action("ConfirmEmail", "Accounts",
                    new {userId = user.Id, token = confirmationToken}, "http");
                await _emailService.SendEmailAsync("andreas3115@gmail.com", "Confirm account", confirmUrl);
                //Send Email

                return true;
            }
            else
            {
                StringBuilder message = new StringBuilder();
                foreach (IdentityError error in result.Errors)
                {
                    message.Append(error + Environment.NewLine);
                }

                throw new Exception(message.ToString());
            }
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded;
            }

            return false;
        }

        public async Task<LoginResult> Login(LoginViewModel loginViewModel)
        {
            User user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user != null)
            {
                bool isPassCorrect = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (isPassCorrect && user.EmailConfirmed)
                {
                    var token = await _tokenProvider.GenerateToken(user);
                    return new LoginResult {IsSuccessful = true, Token = token};
                }
            }

            return new LoginResult {IsSuccessful = false, Message = "Invalid email and/or password"};
        }

        public async Task<bool> ForgotPassword(string email, UrlHelper urlHelper)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string confirmUrl = urlHelper.Action("ResetPassword", "Accounts", new {userId = user.Id, token = token},
                    "http");
                await _emailService.SendEmailAsync("andreas3115@gmail.com", "Reset Password", confirmUrl);
            }

            return false;
        }

        public async Task<bool> ResetPassword(ResetPasswordViewModel resetPassViewModel)
        {
            User user = await _userManager.FindByIdAsync(resetPassViewModel.UserId);
            if (user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetPassViewModel.ResetToken,
                    resetPassViewModel.Password);
                return result.Succeeded;
            }

            return false;
        }
    }
}