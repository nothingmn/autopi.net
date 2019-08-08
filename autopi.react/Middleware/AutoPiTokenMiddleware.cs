using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace autopi.react.Middleware
{
    public class AutoPiTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoPiTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool foundAuth = false;
            var headers = context.Request.Headers;
            if (headers != null)
            {

                StringValues authorizationToken;
                headers.TryGetValue("Authorization", out authorizationToken);
                var auth = authorizationToken.FirstOrDefault();
                if (!string.IsNullOrEmpty(auth))
                {
                    context.SetAutoApiAuthToken(auth);
                    foundAuth = true;
                }
            }
            if (!foundAuth)
            {
                var query = context.Request.Query;
                if (query != null)
                {
                    var auth = (from a in query where a.Key.Equals("auth") select a.Value.FirstOrDefault())?.FirstOrDefault();
                    if (!string.IsNullOrEmpty(auth))
                    {
                        context.SetAutoApiAuthToken(auth);
                        foundAuth = true;
                    }
                }
            }
            if (foundAuth)
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            else
            {
                throw new System.Exception("No AutoPi Authorization found.");
            }
        }
    }

    public static class AutoPiTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseAutoPiTokenMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoPiTokenMiddleware>();
        }
    }
}