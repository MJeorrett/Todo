using MediatR;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Todos.Commands.Create
{
    public record CreateTodoCommand : IRequest<int>
    {
        public string Title { get; init; } = null!;
    }

    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateTodoCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var todoEntity = new TodoEntity()
            {
                Title = request.Title,
            };

            _dbContext.Todos.Add(todoEntity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return todoEntity.Id;
        }
    }
}
