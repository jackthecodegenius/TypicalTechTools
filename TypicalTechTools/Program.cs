using TypicalTechTools.Models.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Options;
using TypicalTechTools.Models.Repository;
using System;
using TypicalTechTools.Models.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using TypicalTechTools.Services;
using Ganss.Xss;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<TypicalTechToolsDBContext>(options =>
{
    options.UseSqlServer(connectionString);
}
);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<FileUploaderService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddSession(c =>
{
    c.IdleTimeout = TimeSpan.FromSeconds(120);
    c.Cookie.HttpOnly = true;
    c.Cookie.IsEssential = true;
    //Locks the cookie holding the session ID to only be able to be sent to the site it was
    //created in.
    c.Cookie.SameSite = SameSiteMode.Strict;
    //sets the cookie so that it can only be sent by https if the site sent it to the
    //browser using https
    c.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            //Sets how long the cookie lasts before deleted.
                            options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                            //Allows the timespan to reset when still in use. Only happens if the timer has lapsed over half its time.
                            options.SlidingExpiration = true;
                            //Sets the default redirection locations for failed access attempts
                            options.LoginPath = "/Admin/AdminLogin";
                            options.AccessDeniedPath = "/Admin/AccessDenied";

                        });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<HtmlSanitizer>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TypicalTechToolsDBContext>();
    dbContext.Database.EnsureCreated();  // This will create the database and tables if they don't exist
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; "+
                               "script-src 'self'; " +
                               "style-src 'self' 'unsafe-inline'; " +
                               "connect-src 'self' http://localhost:* wss://localhost:44372; " +
                               "frame-ancestors 'self'; " +
                               "form-action 'self';");


    await next(context);
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
