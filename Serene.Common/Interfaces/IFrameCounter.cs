namespace Serene.Common.Interfaces;

public interface IFrameCounter
{
    long TotalFrames { get; }
    float TotalSeconds { get; }
    float AverageFramesPerSecond { get; }
    float CurrentFramesPerSecond { get; }
    void Update(float deltaTime);
}