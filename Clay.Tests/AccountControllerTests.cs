using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clay.Controllers;
using Clay.Models.Domain;
using Clay.Models.InputModels.Account;
using Clay.Models.ResponseModels;
using Clay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Clay.Tests
{
    public class AccountControllerTests
    {
        private Mock<UserManager<AppIdentityUser>> _userMgr;
        private Mock<SignInManager<AppIdentityUser>> _signInManager;
        private IConfigurationRoot _configuration;
        private TokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = Mock.Of<IUserStore<AppIdentityUser>>();
            _userMgr = new Mock<UserManager<AppIdentityUser>>(userStoreMock, null, null, null, null, null, null, null, null);
            var user = new AppIdentityUser() { Id = "f00", UserName = "f00", Email = "f00@example.com" };
            var tcs = new TaskCompletionSource<AppIdentityUser>();
            tcs.SetResult(user);
            _userMgr.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(tcs.Task);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppIdentityUser>>();

            _signInManager = new Mock<SignInManager<AppIdentityUser>>(_userMgr.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null);

            var myConfiguration = new Dictionary<string, string>
            {
                {"Jwt:Key", "veryVerySecretKey"},
                {"Jwt:Issuer", "http://localhost:63939/"}
            };

            _configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(myConfiguration)
               .Build();

            _tokenService= new TokenService(_configuration["Jwt:Key"], _configuration["Jwt:Issuer"]);

            
        }

        [Test]
        public void Login_Should_Return_Token()
        {
            //Arrange
            _signInManager.Setup(sim =>
                sim.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .Returns(Task.FromResult(SignInResult.Success));



            var target = new AccountController(_userMgr.Object, _signInManager.Object, _configuration);
            

            var roleList = new List<string> {"User"};

            _userMgr.Setup(um => um.GetRolesAsync(It.IsAny<AppIdentityUser>())).Returns(
                async () =>
                {
                    await Task.Yield();
                    return roleList;
                });

            
            //Act
            var loginModel = new LoginModel { Username = "test", Password = "test" };
            var okObjectResult = target.Login(loginModel).Result as OkObjectResult;

            //Assert
            if(okObjectResult==null)
                Assert.True(false);
            var loginResponseModel = okObjectResult.Value as LoginResponseModel;
            Assert.NotNull(loginResponseModel);
            Assert.AreEqual("f00",loginResponseModel.Id);
            Assert.AreEqual("f00",loginResponseModel.Username);
            Assert.IsNotEmpty(loginResponseModel.Token);

        }
    }
}
