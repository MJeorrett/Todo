using System.Security.Claims;
using Todo.Application.Common.Interfaces;

namespace Todo.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        ILogger<CurrentUserService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            _logger.LogInformation("No http context so can't resolve current user id, it will be null.");
            return null;
        }

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("Failed to resolve user id from http context.");
            return "";
        }

        _logger.LogInformation("Successfully retrieved current user from http context.");
        return userId;
    }
}
