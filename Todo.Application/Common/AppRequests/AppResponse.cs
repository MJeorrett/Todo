using System.Text.Json.Serialization;

namespace Todo.Application.Common.AppRequests;

public record AppResponse
{
    [JsonIgnore]
    public int StatusCode { get; init; }

    public string Message { get; init; }

    public AppResponse(int statusCode, string message = "")
    {
        StatusCode = statusCode;
        Message = message;
    }
}

public record AppResponse<T> : AppResponse
{
    public T? Content { get; init; }

    public AppResponse(int statusCode, T? content = default) :
        base(statusCode)
    {
        Content = content;
    }
}
