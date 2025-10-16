using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Exceptions;

namespace WebApplication1.Handlers
{
    public class InvalidCredentialsExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not InvalidCredentialsException)
            {
                return false;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Invalid Credentials",
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
