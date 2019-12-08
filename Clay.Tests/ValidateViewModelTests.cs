using System;
using System.Collections.Generic;
using Clay.Controllers;
using Clay.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace Clay.Tests
{
    public class ValidateViewModelTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Null_Should_Return_BadRequest()
        {
            //Arrange
            var validationFilter = new ValidateViewModel();
            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionArguments = new Dictionary<string, object> { { "test", null } };

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<ClayControllerBase>()
            );
            //Act
            validationFilter.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result as BadRequestObjectResult;
            //Assert
            if(result==null)
                Assert.True(false);
            Assert.AreEqual("Parameter can not be null!", result.Value.ToString());
        }

        [Test]
        public void InValid_Should_Return_BadRequest()
        {
            //Arrange
            var validationFilter = new ValidateViewModel();
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("key","error");

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionArguments = new Dictionary<string, object> { { "test", "test" } };

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<ClayControllerBase>()
            );

            validationFilter.OnActionExecuting(actionExecutingContext);
            //Act
            var result = actionExecutingContext.Result as BadRequestObjectResult;
            //Assert
            if (result == null)
                Assert.True(false);
            Assert.NotNull(result.Value);
        }
    }
}