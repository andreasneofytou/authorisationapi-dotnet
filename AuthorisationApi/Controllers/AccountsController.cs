using System;
using System.Threading.Tasks;
using AuthorisationApi.Models;
using AuthorisationApi.Results;
using AuthorisationApi.Services;
using AuthorisationApi.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AuthorisationApi.Controllers
{
     [Route("api/accounts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountsController : Controller
    {
        private readonly AccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly UrlHelper _urlHelper;
        private readonly RoleManager<Role> _roleManager;

        public AccountsController(AccountService accountService,
            IActionContextAccessor actionContextAccessor,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _urlHelper = new UrlHelper(actionContextAccessor.ActionContext);
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            LoginResult result = await _accountService.Login(loginViewModel);
            if (!result.IsSuccessful)
            {
                return Unauthorized();
            }
            return Ok(result.Token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel registerViewModel)
        {
            try
            {
                await _accountService.Register(registerViewModel, _urlHelper);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var isSuccessful = await _accountService.ConfirmEmail(userId, token);
            return Ok(isSuccessful);
        }

        [AllowAnonymous]
        [HttpGet("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            await _accountService.ForgotPassword(email, _urlHelper);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string userId, string token)
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
        {
            bool result = await _accountService.ResetPassword(resetPasswordViewModel);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to reset password");
            }
        }
    }
}