using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Clay.Filters
{
    public class ValidateViewModel : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.Any(kv => kv.Value == null))
            {
                actionContext.Result = new BadRequestObjectResult("Parameter can not be null!");
            }

            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
            }
        }
    }
}