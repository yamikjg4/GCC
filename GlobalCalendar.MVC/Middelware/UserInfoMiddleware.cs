using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class UserInfoMiddleware
{
    private readonly RequestDelegate _next;

    public UserInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string empCode = context.Session.GetString("empCode");

        // Exclude the login page from redirection
        if (context.Request.Path != "/GlobalCalendar/Login/Index" && string.IsNullOrEmpty(empCode))
        {
            // Redirect to login page
            context.Response.Redirect("/GlobalCalendar/Login/Index");
            return;
        }
        /*if (context.Request.Path != "/Login/Index" && string.IsNullOrEmpty(empCode))
        {
            // Redirect to login page
            context.Response.Redirect("/Login/Index");
            return;
        }*/
        await _next(context);
    }
}
