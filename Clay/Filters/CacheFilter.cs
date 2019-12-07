using System;
using System.Threading.Tasks;
using Clay.Constants;
using Clay.Controllers;
using Clay.Data.Pagination;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Clay.Filters
{
    public class CustomCacheActionFilter : ActionFilterAttribute
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        public CustomCacheActionFilter(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var defaultPageModel = new PagedModel();
            var actionName = context.RouteData.Values["action"] as string;
            var pagedModel = context.ActionArguments["pagedModel"] as PagedModel;

            if (!string.IsNullOrEmpty(actionName))
            {
                string cacheKey;
                if (pagedModel == null || (pagedModel.Page == defaultPageModel.Page && pagedModel.PageSize == defaultPageModel.PageSize))
                {
                    cacheKey = actionName;
                }
                else
                {
                    cacheKey = actionName + CacheKeys.DELIMITER + pagedModel.Page + CacheKeys.DELIMITER + pagedModel.PageSize;
                }

                if (_cache.TryGetValue(cacheKey, out var attempts))
                    context.Result = new JsonResult(attempts);

            }
        }
        
    }
}