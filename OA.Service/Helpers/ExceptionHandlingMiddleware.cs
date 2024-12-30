using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OA.Domain.VModels;
using System.Net;

namespace OA.Service.Helpers
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;
            switch (exception)
            {
                case UnauthorizedAccessException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case ForbiddenException _:
                    code = HttpStatusCode.Forbidden;
                    break;
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case BadRequestException _:
                    code = HttpStatusCode.BadRequest;
                    break;
                case ConflictException _:
                    code = HttpStatusCode.Conflict;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            };

            var result = ErrorDictionary.Values[(int)code];
            result.Detail = exception.Message;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            _logger.LogError(exception, exception.ToString());

            return context.Response.WriteAsJsonAsync(result);
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {

        }
    }

}
