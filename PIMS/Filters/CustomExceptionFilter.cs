using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PIMS.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log exception details
            _logger.LogError(context.Exception, "An error occurred.");

            // Handle the exception and return a custom response
            context.Result = new ObjectResult(new { message = "An error occurred. Please try again later." })
            {
                StatusCode = 500 // Internal Server Error
            };
        }
    }
}
