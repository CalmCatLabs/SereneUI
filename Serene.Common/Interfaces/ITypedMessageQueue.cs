namespace Serene.Common.Interfaces;

public interface ITypedMessageQueue
{
    void Enqueue<TMessage>(TMessage message);

    bool TryDequeue<TMessage>(out TMessage? message);
}
