using MediatR;

namespace Todo.Application.Common.AppRequests
{
    public interface IAppRequest<T> : IRequest<AppResponse<T>>
    {
    }

    public interface IAppRequest : IRequest<AppResponse>
    {
    }
}
