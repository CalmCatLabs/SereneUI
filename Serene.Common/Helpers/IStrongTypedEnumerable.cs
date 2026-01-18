using System.Collections.Generic;

namespace SereneHorizons.Common.Helpers.Interfaces;

public interface IStrongTypedEnumerable<T> where T : IStrongTypedEnumerable<T>
{
    int Value { get; }
    string Name { get; }

    static abstract IReadOnlyCollection<T> List { get; }

    static abstract T FromValue(int value);
    static abstract T FromName(string? name);
}