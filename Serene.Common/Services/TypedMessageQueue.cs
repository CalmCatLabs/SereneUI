using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Serene.Common.Interfaces;

namespace SereneHorizons.Common.Services;

public class TypedMessageQueue : ITypedMessageQueue
{
    private readonly Dictionary<Type, Queue<object>> _messageQueues = [];
    private readonly Lock _lock = new();

    /// <summary>
    /// Method to enqueue a TMessage.
    /// </summary>
    /// <param name="message">TMessage message to enqueue into the queue.</param>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public void Enqueue<TMessage>(TMessage message)
    {
        lock (_lock)
        {
            if (!_messageQueues.TryGetValue(typeof(TMessage), out var queue))
            {
                queue = new Queue<object>();
                _messageQueues.Add(typeof(TMessage), queue);
            }
            queue.Enqueue(message!);
        }
    }
    
    /// <summary>
    /// Method to try dequeueing a message from the queue.
    /// </summary>
    /// <param name="message">TMessage message to enqueue into the queue.</param>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>true if a message was found otherwise false.</returns>
    public bool TryDequeue<TMessage>(out TMessage? message)
    {
        lock (_lock)
        {
            if (_messageQueues.TryGetValue(typeof(TMessage), out var queue) && queue.Count > 0)
            {
                message = (TMessage?)queue.Dequeue();
                return true;
            }
        }
        message = default;
        return false;
    }
}
