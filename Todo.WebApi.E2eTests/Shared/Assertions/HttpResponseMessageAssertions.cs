using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Models;
using Todo.Domain.Common;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Shared.Assertions;

public static class HttpResponseMessageExtensions
{
    public static HttpResponseMessageAssertions Should(this HttpResponseMessage instance)
    {
        return new HttpResponseMessageAssertions(instance);
    }
}

public class HttpResponseMessageAssertions : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>
{
    protected override string Identifier => "response";

    public HttpResponseMessageAssertions(HttpResponseMessage subject) : base(subject)
    {
    }

    public async Task<AndConstraint<HttpResponseMessageAssertions>> BeStatusCode(int expectedStatusCode)
    {
        var responseContent = expectedStatusCode != (int)Subject.StatusCode ?
            await Subject.Content.ReadAsStringAsync() :
            "";

        Execute.Assertion
            .ForCondition((int)Subject.StatusCode == expectedStatusCode)
            .FailWith($"Expected status code {expectedStatusCode} but recieved status code {(int)Subject.StatusCode} with " +
                (string.IsNullOrEmpty(responseContent) ?
                    "no content." :
                    $"content:\n{responseContent.Replace("{", "{{").Replace("}", "}}")}"));

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    private record ErrorResponse
    {
        public record Error
        {
            public string Name { get; init; } = "";
        }

        public List<Error> Errors { get; init; } = new();
    }

    public async Task<AndConstraint<HttpResponseMessageAssertions>> BeStatusCode400WithErrorForField(string expectedErrorField)
    {
        var responseContent = await Subject.Content.ReadAsStringAsync();

        Execute.Assertion
            .ForCondition((int)Subject.StatusCode == 400)
            .FailWith($"Expected status code 400 but recieved status code {(int)Subject.StatusCode} with " +
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

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public async Task<AndWhichConstraint<HttpResponseMessageAssertions, PaginatedListResponse<T>>> ContainPaginatedListOf<T>()
    {
        await Subject.Should().BeStatusCode(200);

        var (parseSuccess, paginatedListResponse) = await Subject.TryReadResponseContentAs<PaginatedListResponse<T>>();

        Execute.Assertion
            .ForCondition(parseSuccess)
            .FailWith($"Expected content to be parseable as a paginated list response of type {typeof(T)}");

        return new AndWhichConstraint<HttpResponseMessageAssertions, PaginatedListResponse<T>>(this, paginatedListResponse!);
    }
}
