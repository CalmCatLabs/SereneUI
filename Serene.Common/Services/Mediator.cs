using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serene.Common.Interfaces;

namespace Serene.Common.Services;

public readonly struct Unit
{
    public static readonly Unit Value = new();
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TResponse Send<TResponse>(ICommand<TResponse> command)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        // Handler aus DI ziehen
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // dynamic, damit wir nicht tausend Generics bauen müssen
        return ((dynamic)handler).Handle((dynamic)command);
    }

    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        // Handler aus DI ziehen
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // dynamic, damit wir nicht tausend Generics bauen müssen
        return ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);
    }
}
