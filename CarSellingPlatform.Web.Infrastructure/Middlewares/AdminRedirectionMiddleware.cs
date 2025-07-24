using Microsoft.AspNetCore.Http;

namespace CarSellingPlatform.Web.Infrastructure.Middlewares;

public class AdminRedirectionMiddleware
{
    private readonly RequestDelegate next;

    public AdminRedirectionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            if (context.Request.Path == "/" && context.User.IsInRole("Admin"))
            {
                context.Response.Redirect("/Admin");
                return;
            }
        }
        await this.next(context);
    }
}