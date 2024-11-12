using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Services.Exceptions;
using System.Text.Json;
using static Services.Exceptions.InventoryAlreadyExistsException;

namespace PIMS.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case InventoryAlreadyExistsException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case InventoryNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case InvalidTransactionException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message= exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)status;
            var result = System.Text.Json.JsonSerializer.Serialize(new { message });
            return context.Response.WriteAsync(result);
        }        
    }
}
