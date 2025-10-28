using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace hehehe.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();

            var allowedPaths = new[]
            {
                "/auth", "/css", "/js", "/images", "/lib", "/favicon.ico"
            };

            if (allowedPaths.Any(p => path.StartsWith(p)) || path == "/")
            {
                await _next(context);
                return;
            }
            
            if (context.Session.GetString("MaNhapHoc") == null)
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }

            await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}