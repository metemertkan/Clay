using System;
using System.Linq;
using Clay.Data.Pagination;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Clay.Filters
{
    public class PaginationCorrection : ActionFilterAttribute
    {
        public string ParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(ParamName))
                return;

            if (!context.ActionArguments.ContainsKey(ParamName))
                return;

            var pagedModel = context.ActionArguments[ParamName] as PagedModel;
            if (pagedModel == null)
            {
                context.ActionArguments[ParamName] = new PagedModel();
            }
        }
    }
}