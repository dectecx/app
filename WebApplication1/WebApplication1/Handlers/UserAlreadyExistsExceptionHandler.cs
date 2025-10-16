using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Exceptions;

namespace WebApplication1.Handlers
{
    public class UserAlreadyExistsExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not UserAlreadyExistsException)
            {
                return false;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "User Already Exists",
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
