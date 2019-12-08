using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Clay.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionFilter>();
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception.Message);
            context.Result = new StatusCodeResult(500);
            return base.OnExceptionAsync(context);
        }
    }
}