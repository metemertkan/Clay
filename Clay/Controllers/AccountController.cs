using Clay.Models.Account;
using Clay.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private UserManager<AppIdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<AppIdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            IActionResult response = Unauthorized();
            if (!ModelState.IsValid)
                return response;

            var result =  signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).Result;

            if (!result.Succeeded)
                return response;

            var logedinUser = userManager.FindByNameAsync(model.Username).Result;

           // var tokenService = new TokenService();

           // var token = tokenService.GenerateToken(model.Username);
            response = Ok(new { id = logedinUser.Id, userName = logedinUser.UserName/*, token = token*/ });

            return response;
        }
    }
}