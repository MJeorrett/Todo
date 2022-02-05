﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Todo.Application.Common.Interfaces;
using Todo.Infrastructure.DateTimes;
using Todo.Infrastructure.Identity;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Persistence.Seeding;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddDateTimes();
        services.AddIdentity(configuration, environment.EnvironmentName);
        services.AddPersistence(configuration);

        if (environment.IsDevelopment())
        {
            services.AddSeeders();
        }

        return services;
    }

    private static void AddDateTimes(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
    }

    private static void AddIdentity(
        this IServiceCollection services,
        IConfiguration configuration,
        string environmentName)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.AllowClientCredentialsFlow();
                options.AllowAuthorizationCodeFlow()
                    .RequireProofKeyForCodeExchange();
                options.AllowRefreshTokenFlow();

                options.SetAuthorizationEndpointUris("/connect/authorize");
                options.SetTokenEndpointUris("/connect/token");
                options.SetLogoutEndpointUris("/connect/logout");

                var base64SigningKey = configuration["Identity:Base64SigningKey"];
                var signingKeyBytes = Convert.FromBase64String(configuration["Identity:Base64SigningKey"]);
                var signingKeyRsa = RSA.Create();
                signingKeyRsa.ImportPkcs8PrivateKey(signingKeyBytes, out var _);
                var signingKey = new RsaSecurityKey(signingKeyRsa);

                options.AddSigningKey(signingKey);
                options.AddEphemeralEncryptionKey();

                options.DisableAccessTokenEncryption();

                options.RegisterScopes("api");

                var openiddictAspnetCoreBuilder = options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough();

                if (environmentName == "Test")
                {
                    openiddictAspnetCoreBuilder.DisableTransportSecurityRequirement();
                }
            });
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer");

        if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Missing configuration for SqlServer ConnectionString.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.UseOpenIddict();
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddSeeders(this IServiceCollection services)
    {
        services.AddHostedService<AuthDataSeeder>();
    }
}
