using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PIMS.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        //private readonly ILogger<CustomExceptionFilter> _logger;

        //public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        //{
        //    _logger = logger;
        //}

        public void OnException(ExceptionContext context)
       {
        //    // Log exception details only
        //    _logger.LogError(context.Exception, "An error occurred in an MVC action.");
        //    // Do not set context.Result here to avoid conflicting with middleware
        }
    }
}
