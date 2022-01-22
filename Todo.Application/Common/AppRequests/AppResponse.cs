namespace Todo.Application.Common.AppRequests;

public record AppResponse
{
    public int StatusCode { get; init; }

    public string? Message { get; init; }

    public AppResponse(int statusCode)
    {
        StatusCode = statusCode;
    }
}

public record AppResponse<T> : AppResponse
{
    public T Content { get; init; }

    public AppResponse(T content, int statusCode) :
        base(statusCode)
    {
        Content = content;
    }
}
