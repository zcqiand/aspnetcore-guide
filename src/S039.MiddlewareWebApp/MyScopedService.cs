using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MiddlewareWebApp07
{
    public class MyCustomMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<MyCustomMiddleware> logger;

        public MyCustomMiddleware(RequestDelegate next, ILogger<MyCustomMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            logger.LogInformation($"{context.Request.Method} {context.Request.Path}");
            await next(context);
        }
    }

    public static class MyCustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustom(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyCustomMiddleware>();
        }
    }
}
