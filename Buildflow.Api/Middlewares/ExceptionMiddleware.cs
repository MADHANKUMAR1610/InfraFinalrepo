//using Microsoft.AspNetCore.Http;
//using Serilog;
//using System.Net;

//namespace Buildflow.Api.Middlewares
//{
//    public class ExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public ExceptionMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext httpContext)
//        {
//            try
//            {
//                await _next(httpContext);
//            }
//            catch (Exception ex)
//            {
//                Log.Error($"Something went wrong: {ex}");
//                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                await httpContext.Response.WriteAsync("Internal Server Error.");
//            }
//        }
//    }

//    public static class ExceptionMiddlewareExtensions
//    {
//        public static void ExceptionMiddleware(this IApplicationBuilder app)
//        {
//            app.UseMiddleware<ExceptionMiddleware>();
//        }
//    }
//}


using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;

namespace Buildflow.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //// Log the detailed exception message
                //var detailedMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                //Log.Error($"Something went wrong: {detailedMessage}");

                //// Set status code
                //httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //// Return detailed error message in response
                //await httpContext.Response.WriteAsync($"Internal Server Error: {detailedMessage}");

                // Log the full exception details
                Log.Error($"An error occurred: {ex.Message}\n{ex.StackTrace}");

                // Optionally log inner exceptions for more details
                if (ex.InnerException != null)
                {
                    Log.Error($"Inner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}");
                }

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static void ExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
