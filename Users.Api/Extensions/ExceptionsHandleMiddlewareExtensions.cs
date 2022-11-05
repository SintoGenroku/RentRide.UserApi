using Users.Api.Middlewares;

namespace Users.Api.Extensions;

public static class ExceptionsHandleMiddlewareExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandleMiddleware>();
    }
}