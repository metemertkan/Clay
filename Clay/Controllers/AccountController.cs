using Clay.Models.Account;
using Clay.Models.Domain;
using Clay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Clay.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppIdentityUser> userManager;
        private SignInManager<AppIdentityUser> signInManager;
        private IConfiguration _configuration;

        public AccountController(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            IActionResult response = Unauthorized();
            if (!ModelState.IsValid)
                return response;

            var result = signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).Result;

            if (!result.Succeeded)
                return response;

            var logedinUser = userManager.FindByNameAsync(model.Username).Result;

            var tokenService = new TokenService(
                _configuration.GetSection("Jwt").GetSection("Key").Value,
                _configuration.GetSection("Jwt").GetSection("Issuer").Value
            );

            var token = tokenService.GenerateToken(model.Username);
            response = Ok(new { id = logedinUser.Id, userName = logedinUser.UserName, token = token });

            return response;
        }
    }
}