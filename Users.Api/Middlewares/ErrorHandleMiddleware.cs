using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.WebUtilities;
using RentRide.Common.Exceptions;
using Users.Api.Models.Responses.Errors;

namespace Users.Api.Middlewares;

public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
        
        public ErrorHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(BadRequestException ex)
            {
                var error = HandleError(ex, StatusCodes.Status400BadRequest);
                await SendErrorAsync(context, error);
            }
            catch(ForbiddenException ex)
            {
                var error = HandleError(ex, StatusCodes.Status403Forbidden);
                await SendErrorAsync(context, error);
            }
            catch(NotFoundException ex)
            {
                var error = HandleError(ex, StatusCodes.Status404NotFound);
                await SendErrorAsync(context, error);
            }
            catch(UnauthorizedException ex)
            {
                var error = HandleError(ex, StatusCodes.Status401Unauthorized);
                await SendErrorAsync(context, error);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                var error = HandleError(ex, StatusCodes.Status400BadRequest);
                await SendErrorAsync(context, error);
            }
            catch(Exception ex)
            {
                var error = HandleError(ex, StatusCodes.Status500InternalServerError);
                await SendErrorAsync(context, error);
            }
        }


        private static ErrorResponse HandleError(Exception ex, int statusCode)
        {
            var errorResponse = new ErrorResponse
            {
                Details = ex.Message,
                Message = ReasonPhrases.GetReasonPhrase(statusCode),
                StatusCode = statusCode

            };

            if (ex.InnerException != null)
            {
                errorResponse.InnerError = HandleError(ex.InnerException, statusCode);
            }

            return errorResponse;
        }

        private static async Task SendErrorAsync(HttpContext context, ErrorResponse errorResponse)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;
            var jsonResponse = JsonSerializer.Serialize(errorResponse, options);

            await context.Response.WriteAsync(jsonResponse);
        }
}