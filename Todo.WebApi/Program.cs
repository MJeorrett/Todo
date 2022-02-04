using Microsoft.AspNetCore.Authentication.Cookies;
using Todo.Application;
using Todo.Infrastructure;
using Todo.WebApi;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddInfrastructure(builder.Configuration, builder.Environment.EnvironmentName);
services.AddApplication();

services.AddControllersWithViews()
    .AddApplicationFluentValidation();


services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.AddHostedService<TestDataSeeder>();

var app = builder.Build();

app.UseSwaggerUI();
app.UseSwagger();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseEndpoints(endpoints =>
    endpoints.MapDefaultControllerRoute());

app.Run();

public partial class Program { }