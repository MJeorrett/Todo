namespace Todo.WebApi.E2eTests.Shared.Models;

public record ClientApplicationDetails
{
    public string ClientId { get; init; } = "e2e-default-test-client";
    public string RedirectUri { get; init; } = "http://localhost:3123";
    public string Scope { get; init; } = "api";
}
