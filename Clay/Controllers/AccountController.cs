using System.Linq;
using System.Threading.Tasks;
using Clay.Filters;
using Clay.Models.InputModels.Account;
using Clay.Models.Domain;
using Clay.Models.ResponseModels;
using Clay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Clay.Controllers
{
    public class AccountController : ClayControllerBase
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
        [ValidateViewModel]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> Login(LoginModel model)
        {
            IActionResult response = Unauthorized();
           
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (!result.Succeeded)
                return response;

            var logedinUser = await userManager.FindByNameAsync(model.Username);

            var tokenService = new TokenService(
                _configuration.GetSection("Jwt").GetSection("Key").Value,
                _configuration.GetSection("Jwt").GetSection("Issuer").Value
            );
            var role = (await userManager.GetRolesAsync(logedinUser)).FirstOrDefault();
            var token = tokenService.GenerateToken(model.Username,logedinUser.Id, role);
            var responseModel = new LoginResponseModel { Id = logedinUser.Id, Username = logedinUser.UserName, Token = token };
            response = Ok(responseModel);

            return response;
        }
    }
}