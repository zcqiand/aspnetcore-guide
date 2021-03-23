using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareWebApp06
{
    public class Startup
    {
        private static void HandleTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Test 1");
            });
        }

        private static void HandleTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Test 2");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/test1", HandleTest1);

            app.MapWhen(context => context.Request.Query.ContainsKey("test2"),
                    HandleTest2);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Test 3");
            });
        }
    }
}
