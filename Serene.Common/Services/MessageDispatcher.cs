using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serene.Common.Interfaces;

namespace Serene.Common.Services;

public class MessageDispatcher(ITypedMessageQueue messageQueue) : IMessageDispatcher
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly Lock _lock = new();
    
    /// <summary>
    /// Add a handler action to the handlers list of a given message type.
    /// </summary>
    /// <param name="handler">Action handler to register.</param>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public void AddHandler<TMessage>(Func<TMessage, Task> handler) where TMessage : class
    {
        var messageType = typeof(TMessage);

        lock (_lock)
        {
            if (!_handlers.TryGetValue(messageType, out var handlers))
            {
                handlers = [];
                _handlers.Add(messageType, handlers);
            }

            handlers.Add(handler);

        }
        ProcessQueuedMessages<TMessage>();
    }

    private void ProcessQueuedMessages<TMessage>() where TMessage : class
    {
        while (messageQueue.TryDequeue(out TMessage? message) && message != null)
        {
            Task.Run(() => Dispatch(message));
        }
    }

    /// <summary>
    /// Entfernt einen spezifischen Handler
    /// </summary>
    public void RemoveHandler<TMessage>(Func<TMessage, Task> handler) where TMessage : class
    {
        var messageType = typeof(TMessage);

        lock (_lock)
        {
            if (!_handlers.TryGetValue(messageType, out var handlerList)) return;

            handlerList.Remove(handler);
            
            if (handlerList.Count == 0)
            {
                _handlers.Remove(messageType);
            }

        }
    }
    
    /// <summary>
    /// Versendet eine Message an alle registrierten Handler
    /// </summary>
    public void Dispatch<TMessage>(TMessage message) where TMessage : class
    {
        var messageType = typeof(TMessage);
        List<Delegate> handlersToNotify = [];

        lock (_lock)
        {
            if (!_handlers.TryGetValue(messageType, out var messageHandlers))
            {
                // Queue Message.
                messageQueue.Enqueue(message);
                return;
            }
            handlersToNotify = messageHandlers.ToList();
        }
        
        foreach (var handler in handlersToNotify)
        {
            if (handler is not Func<TMessage, Task> typedHandler) continue;
            
            try
            {
                _ = typedHandler(message);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Handler exception for message type {typeof(TMessage).Name}: {e}");
            }
        }
    }
}