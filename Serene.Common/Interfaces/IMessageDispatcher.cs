using System;
using System.Threading.Tasks;

namespace Serene.Common.Interfaces;

public interface IMessageDispatcher
{
    void AddHandler<TMessage>(Func<TMessage, Task> handler) where TMessage : class;
    void RemoveHandler<TMessage>(Func<TMessage, Task> handler) where TMessage : class;
    void Dispatch<TMessage>(TMessage message) where TMessage : class;
}