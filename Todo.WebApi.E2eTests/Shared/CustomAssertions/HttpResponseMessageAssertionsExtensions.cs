using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Shared.Assertions;

public static class HttpResponseMessageAssertionsExtensions
{
    public static async Task<AndConstraint<HttpResponseMessageAssertions>> HaveStatusCode(this HttpResponseMessageAssertions target, int expectedStatusCode)
    {
        var subject = target.Subject;

        var responseContent = expectedStatusCode != (int)subject.StatusCode ?
            await subject.Content.ReadAsStringAsync() :
            "";

        Execute.Assertion
            .ForCondition((int)subject.StatusCode == expectedStatusCode)
            .FailWith($"Expected status code {expectedStatusCode} but recieved status code {(int)subject.StatusCode} with " +
                (string.IsNullOrEmpty(responseContent) ?
                    "no content." :
                    $"content:\n{responseContent.Replace("{", "{{").Replace("}", "}}")}"));

        return new AndConstraint<HttpResponseMessageAssertions>(target);
    }

    private record ErrorResponse
    {
        public record Error
        {
            public string Name { get; init; } = "";
        }

        public List<Error> Errors { get; init; } = new();
    }

    public static async Task<AndConstraint<HttpResponseMessageAssertions>> HaveStatusCode400WithErrorForField(this HttpResponseMessageAssertions target, string expectedErrorField)
    {
        var subject = target.Subject;

        var responseContent = await subject.Content.ReadAsStringAsync();

        Execute.Assertion
            .ForCondition((int)subject.StatusCode == 400)
            .FailWith($"Expected status code 400 but recieved status code {(int)subject.StatusCode} with " +
                (string.IsNullOrEmpty(responseContent) ?
                    "no content." :
                    $"content:\n{responseContent.Replace("{", "{{").Replace("}", "}}")}"))
            .Then
            .Given(() =>
            {
                try
                {
                    var errorResponse = JsonDocument.Parse(responseContent);

                    return Tuple.Create(true, (JsonDocument?)errorResponse);
                }
                catch (Exception)
                {
                    return Tuple.Create<bool, JsonDocument?>(false, null);
                }
            })
            .ForCondition(deserialisationResult => deserialisationResult.Item1)
            .FailWith("Expected 400 response content to be standard format: {{ errors: [ {{ name: string }} ]}}.")
            .Then
            .ForCondition(deserialisationResult => deserialisationResult.Item2!.RootElement.TryGetProperty("errors", out var errors))
            .FailWith("Expected response content to have key 'errors'.")
            .Then
            .Given(deserialisationResult => deserialisationResult.Item2!.RootElement.GetProperty("errors"))
            .ForCondition(errors => errors.TryGetProperty(expectedErrorField, out var _))
            .FailWith("Expected error for: {0}\nFound error key(s): {0}.",
                _ => expectedErrorField,
                errors => string.Join(",", errors.EnumerateObject().Select(_ => _.Name)));

        return new AndConstraint<HttpResponseMessageAssertions>(target);
    }

    public static async Task<AndWhichConstraint<HttpResponseMessageAssertions, PaginatedListResponse<T>>> ContainPaginatedListOf<T>(this HttpResponseMessageAssertions target)
    {
        var subject = target.Subject;

        await subject.Should().HaveStatusCode(200);

        var (parseSuccess, parsedContent) = await subject.TryReadContentAs<AppResponse<PaginatedListResponse<T>>>();

        Execute.Assertion
            .ForCondition(parseSuccess)
            .FailWith($"Expected content to be parseable as a paginated list response of type {typeof(T)}");

        return new AndWhichConstraint<HttpResponseMessageAssertions, PaginatedListResponse<T>>(target, parsedContent!.Content!);
    }

    public static async Task<AndWhichConstraint<HttpResponseMessageAssertions, AppResponse<T>>> ContainAppResponseOfType<T>(this HttpResponseMessageAssertions target)
    {
        var subject = target.Subject;

        var (parseSuccess, parsedContent) = await subject.TryReadContentAs<AppResponse<T>>();

        Execute.Assertion
            .ForCondition(parseSuccess)
            .FailWith($"Expected content to be parseable as standard app response of type {typeof(T)}");

        return new AndWhichConstraint<HttpResponseMessageAssertions, AppResponse<T>>(target, parsedContent!);
    }
}
