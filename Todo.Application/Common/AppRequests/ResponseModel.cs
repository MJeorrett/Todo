namespace Todo.Application.Common.AppRequests
{
    public record ResponseModel
    {
        public int StatusCode { get; init; }

        public string? Message { get; init; }
    }

    public record ResponseModel<T>
    {
        public int StatusCode { get; init; }

        public T Content { get; init; }

        public string? Message { get; init; }

        public ResponseModel(T content)
        {
            Content = content;
        }
    }
}
