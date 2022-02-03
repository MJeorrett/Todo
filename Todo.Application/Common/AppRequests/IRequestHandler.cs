namespace Todo.Application.Common.AppRequests;

public interface IRequestHandler<TRequest, TResult>
{
    Task<AppResponse<TResult>> Handle(TRequest request, CancellationToken cancellationToken);
}
