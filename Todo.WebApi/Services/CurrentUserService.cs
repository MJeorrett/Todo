using OpenIddict.Abstractions;
using System.Security.Claims;
using Todo.Application.Common.Interfaces;

namespace Todo.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? UserId { get; private set; }

    public CurrentUserService(
        ILogger<CurrentUserService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            logger.LogWarning("No http context so can't resolve current user id, it will be null.");
            return;
        }

        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(OpenIddictConstants.Claims.Subject);

        if (string.IsNullOrEmpty(UserId))
        {
            logger.LogInformation("Failed to resolve user id from http context.");
        }

        logger.LogInformation("Successfully retrieved current user from http context.");
    }
}
