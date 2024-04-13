using Application.Exceptions;
using Common.Responses.Wrappers;
using Ganss.Xss;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApi.Helpers;

namespace WebApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next; //  ASP.NET Core uygulamasında HTTP isteklerini işlemek için kullanılan bir tür
    private readonly ILogger<ErrorHandlingMiddleware> _logger;



    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    private static async Task<Object?> GetRawLog(HttpRequest request)
    {
        Object? log;

        try
        {
            using var streamReader = new StreamReader
                 (request.Body, Encoding.UTF8, leaveOpen: true);
            log = new
            {
                method = request.Method + " " + request.GetDisplayUrl(),
                protocol = request.Protocol,
                headers = request.Headers,
                body = await streamReader.ReadToEndAsync(),
            };
        }
        catch
        {
            log = "Request could not resolved.";
        }


        return log;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestId = Guid.NewGuid();
        LogContext.PushProperty("RequestId", requestId);
        //try
        //{
        //    await _next(httpContext); // middleware ilerleyişini devam ettir.
        //}
        //catch (Exception ex)
        //{
        //    var response = httpContext.Response;
        //    response.ContentType = "application/json";
        //    var responseWrapper = await ResponseWrapper.FailAsync("Bir hata oluştu.");

        //    switch (ex)
        //    {
        //        case CustomValidationException validationException:
        //            response.StatusCode = (int)HttpStatusCode.BadRequest;
        //            break;
        //        default:
        //            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //            break;
        //    }

        //    var result = JsonSerializer.Serialize(responseWrapper);
        //    await response.WriteAsync(result);
        //}
        try
        {
            _logger.LogInformation("Starting request {@RequestId},{@RequestName}, {@DateTime}, {@IpAddress}", requestId,context.Request.Path, DateTime.UtcNow, IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress));
            var response;
            HtmlSanitizer sanitizer = new();

            if (context.Request.Path.Value is not null)
            {
                var path = context.Request.Path.Value;
                var sanitizedPath = sanitizer.Sanitize(path);

                if (path != sanitizedPath)
                {
                    _logger.LogCritical("XSS in request path {@RequestId},{@RequestName}, {@Detected}, {@Request}, {@IpAddress}, {@DateTime}",
                        requestId,
                        context.Request.Path,
                        path,
                        GetRawLog(context.Request).Result,
                        IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress),
                        DateTime.Now);

                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("İşlem Reddedildi"));
                    return;
                }
            }

            if (context.Request.QueryString.Value is not null)
            {
                var query = context.Request.QueryString.Value;
                var sanitizedQuery = sanitizer.Sanitize(query);

                if (query != WebUtility.HtmlDecode(sanitizedQuery))
                {
                    _logger.LogCritical("XSS in request querystring {@RequestId},{@RequestName}, {@Detected}, {@Request}, {@IpAddress}, {@DateTime}",
                        requestId,
                        context.Request.Path,
                        query,
                        GetRawLog(context.Request).Result,
                        IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress),
                        DateTime.Now);


                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("İşlem Reddedildi"));
                    return;
                }
            }

            context.Request.EnableBuffering();

            using (var streamReader = new StreamReader
              (context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                if (!context.Request!.Path.Value.Contains("api/mail/send"))
                {
                    var body = Regex.Unescape((await streamReader.ReadToEndAsync()).Replace("\r\n", "\n"));
                    var sanitizedBody = Regex.Unescape(sanitizer.Sanitize(body));

                    if (body != WebUtility.HtmlDecode(sanitizedBody))
                    {
                        _logger.LogCritical("XSS in request body {@RequestId},{@RequestName}, {@Detected}, {@Request}, {@IpAddress}, {@DateTime}",
                            requestId,
                            context.Request.Path,
                            body,
                            GetRawLog(context.Request).Result,
                            IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress),
                            DateTime.Now);
                      
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("İşlem Reddedildi"));
                        return;
                    }
                }
            }

            context.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(context);

            _logger.LogInformation("Completed request {@RequestId},{@RequestName}, {@DateTime}, {@IpAddress}",
                requestId,
                context.Request.Path,
                DateTime.UtcNow,
                IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress));
        }
        catch (Exception e)
        {
            _logger.LogError("Completed request with exception {@RequestId},{@RequestName}, {@Message}, {@Exception}, {@Request}, {@IpAddress}, {@DateTime}",
                requestId,
                context.Request.Path,
                e.Message,
                e,
                GetRawLog(context.Request),
                IpAddressHelper.GetIpAddress(context.Connection.RemoteIpAddress),
                DateTime.Now);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ResponseWrapper.Fail($"Bir hata oluştu. Hata takip numaranız : {requestId}"));
        }

    }
}
