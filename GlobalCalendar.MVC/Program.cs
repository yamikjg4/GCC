using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using Chat.Web.Data;
using Chat.Web.Helpers;
using Chat.Web.Hubs;
using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

//Session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
/*builder.Services.AddDistributedMemoryCache();*/ // Use an appropriate distributed cache in a production environment
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust the timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("TBSConnStr")));
builder.Services.AddHangfire(config => config.UseSqlServerStorage("Data Source=SELPUNNETUTDB01\\NETUAT, 1433;Initial Catalog=Global_Calender;User ID=uat;Password=Suzlon@123"));
builder.Services.AddHangfireServer();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<TBSDAL>();
builder.Services.AddSingleton<CommonRepo>();
/*builder.Services.AddSingleton<EmailSenderService>();*/

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSignalR();
builder.Services.AddTransient<IFileValidator, FileValidator>();

var app = builder.Build();

/*await app.RunAsync();*/
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseSession();
app.UseAuthentication();


// Register your custom middleware here
/*app.UseMiddleware<UserInfoMiddleware>();*/

app.UseRouting();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
   /* endpoints.MapRazorPages();*/
    endpoints.MapControllers();
    //endpoints.MapHub<ChatHub>("/chatHub"); // Map the ChatHub to the /chat endpoint
    // Map the ChatHub to the /chat endpoint
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

});

app.Run();





