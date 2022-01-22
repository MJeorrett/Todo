using MediatR;

namespace Todo.Application.Common.AppRequests
{
    public interface IAppRequestHandler<TRequest, TResponse> :
        IRequestHandler<TRequest, AppResponse<TResponse>>
        where TRequest : IRequest<AppResponse<TResponse>>
    {

    }
}
