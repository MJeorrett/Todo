using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Todo.Application;
using Todo.Application.Common.Interfaces;
using Todo.Infrastructure;
using Todo.Infrastructure.Persistence;
using Todo.WebApi.Extensions;
using Todo.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

#region ConfigureServices

services.AddInfrastructure(builder.Configuration, builder.Environment);
services.AddApplication();

services.AddControllersWithViews()
    .AddApplicationFluentValidation();

AddCors();
AddAuthentication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.AddScoped<ICurrentUserService, CurrentUserService>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Test")
{
    EnsureDatabaseCreatedAndMigrated();
}

#region Configure

app.UseSwaggerUI();
app.UseSwagger();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
    endpoints.MapDefaultControllerRoute());

#endregion

app.Run();

#region Helpers

void AddCors() => services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

void AddAuthentication() 
{
    var identityOptions = configuration.LoadAppIdentityOptions();

    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = identityOptions.SigningKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidIssuer = identityOptions.Issuer,
        };
    });
}

void EnsureDatabaseCreatedAndMigrated()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

#endregion

public partial class Program { }
