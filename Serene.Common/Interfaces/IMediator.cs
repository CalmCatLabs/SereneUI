using System.Threading;
using System.Threading.Tasks;

namespace Serene.Common.Interfaces;

public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default);

    TResponse Send<TResponse>(
        ICommand<TResponse> command);
}
