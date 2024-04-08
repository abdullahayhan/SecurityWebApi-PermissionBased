using Application.Exceptions;
using Common.Responses;
using Common.Responses.Wrappers;
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
            var responseWrapper = await ResponseWrapper.FailAsync("Bir hata oluştu.");

            switch (ex)
            {
                case CustomValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(responseWrapper);
            await response.WriteAsync(result);
        }
    }
}
