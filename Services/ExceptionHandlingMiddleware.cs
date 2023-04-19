using System.Net;
using System.Text.Json;
using MongoDB.Bson.IO;

namespace Journal.Services;

public class ExceptionHandlingMiddleware : IMiddleware
{

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            message = exception.Message,
            statusCode = httpContext.Response.StatusCode
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}