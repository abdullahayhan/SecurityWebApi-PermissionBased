using Application.Exceptions;
using Common.Responses;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next; //  ASP.NET Core uygulamasında HTTP isteklerini işlemek için kullanılan bir tür

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next; 
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext); // middleware ilerleyişini devam ettir.
        }
        catch (Exception ex)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";
            Error error = new();

            switch (ex)
            {
                case CustomValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    error.Description = validationException.Description;
                    error.ErrorsMessage = validationException.ErrorsMessage;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error.Description = ex.Message; // ?? 
                    break;
            }

            var result = JsonSerializer.Serialize(error);
            await response.WriteAsync(result);
        }
    }
}
