using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Authentication
{
    public class AuthenticationAttribute
    {
        private const string AuthenticationHeaderName = "Authentication";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthenticationHeaderName, out var potentialKey))
            {
                context.Result = new ContentResult() { Content = HttpStatusCode.Unauthorized.ToString(), StatusCode = (int)HttpStatusCode.Unauthorized };
                return;
            }
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = config.GetValue<string>(key: AuthenticationHeaderName);

            if (!apiKey.Equals(potentialKey))
            {
                context.Result = new ContentResult() { Content = HttpStatusCode.Unauthorized.ToString(), StatusCode = (int)HttpStatusCode.Unauthorized };
                return;
            }
            await next();
        }
    }
}
