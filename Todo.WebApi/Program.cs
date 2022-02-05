using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Todo.Application;
using Todo.Application.Common.Interfaces;
using Todo.Infrastructure;
using Todo.WebApi;
using Todo.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddInfrastructure(builder.Configuration, builder.Environment);
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

var base64SigningKey = configuration["Identity:Base64SigningKey"];
var signingKeyBytes = Convert.FromBase64String(configuration["Identity:Base64SigningKey"]);
var signingKeyRsa = RSA.Create();
signingKeyRsa.ImportPkcs8PrivateKey(signingKeyBytes, out var _);
var signingKey = new RsaSecurityKey(signingKeyRsa);

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
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidIssuer = configuration["Identity:Issuer"],
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

app.UseSwaggerUI();
app.UseSwagger();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
    endpoints.MapDefaultControllerRoute());

app.Run();

public partial class Program { }