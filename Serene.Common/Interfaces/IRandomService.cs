namespace Serene.Common.Interfaces;

public interface IRandomService
{
    public void SetSeed(int seed);

    public int NextInt(int minValue, int maxValue);
    
    public int NextInt(int maxValue) => NextInt(0, maxValue);

    public double NextDouble();
}