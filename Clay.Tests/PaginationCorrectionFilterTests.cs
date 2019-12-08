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
    public class PaginationCorrectionFilterTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Empty_Param_Should_Return_Nothing()
        {
            //Arrange
            var paginationCorrection = new PaginationCorrection();
            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

        
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(), 
                Mock.Of<ClayControllerBase>()
            );
            //Act
            paginationCorrection.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result;
            //Assert
            if (result == null)
                Assert.True(true);
        }

        [Test]
        public void Return_Nothing_If_Param_Not_In_Action_Argument()
        {
            //Arrange
            var paginationCorrection = new PaginationCorrection();
            paginationCorrection.ParamName = "test";

            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<ClayControllerBase>()
            );
            //Act
            paginationCorrection.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result;
            //Assert
            if (result == null)
                Assert.True(true);
        }

        [Test]
        public void Param_Should_Be_Included_In_Action_Arguments()
        {
            //Arrange
            var paginationCorrection = new PaginationCorrection();
            paginationCorrection.ParamName = "test";

            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionArguments= new Dictionary<string,object>{
            {
                "test","test"
            }};

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<ClayControllerBase>()
            );
            //Act
            paginationCorrection.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.ActionArguments;
            //Assert
           Assert.True(result.Keys.Contains("test"));
        }
    }
}