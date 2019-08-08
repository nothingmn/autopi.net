using Microsoft.AspNetCore.Http;

namespace autopi.react.Middleware
{
    public static class AutoAPIWebExtensions
    {
        public static void SetAutoApiAuthToken(this HttpContext context, string auth)
        {
            if (!auth.StartsWith("Bearer ")) auth = $"Bearer {auth}";
            context.Items["AutoPi.Auth"] = auth;
        }
        public static string GetAutoApiAuthToken(this HttpContext context)
        {
            if (context.Items.ContainsKey("AutoPi.Auth")) return context.Items["AutoPi.Auth"] as string;
            return null;
        }
    }
}