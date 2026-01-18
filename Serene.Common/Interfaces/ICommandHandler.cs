using System.Threading;
using System.Threading.Tasks;

namespace Serene.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
    TResponse Handle(TCommand command);
}
