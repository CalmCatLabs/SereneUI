using System;
using System.Collections.Generic;
using System.Linq;
using SereneHorizons.Common.Helpers.Interfaces;

namespace Serene.Common.Helpers;

public class StrongTypedEnumerable<T> : IStrongTypedEnumerable<T> where T : StrongTypedEnumerable<T>
{
    private static int valueCount = 0;
    public int Value { get; }
    public string Name { get; }
    
    public static IReadOnlyCollection<T> List
    {
        get
        {
            var resultCollection = new List<T>();
            foreach (var value in _lookup)
            {
                resultCollection.Add(StrongTypedEnumerable<T>.FromValue(value.Key));
            }
            return resultCollection.AsReadOnly();
        }
    }
    
    // ReSharper disable once InconsistentNaming
    private static readonly Dictionary<int, T> _lookup = [];
    
    protected StrongTypedEnumerable(int value, string name)
    {
        Value = value;
        Name = name;
        if (!_lookup.TryAdd(value, (T)this))
            throw new InvalidOperationException($"Duplicate value {value} in {typeof(T).Name}");
    }
    
    protected StrongTypedEnumerable(string name)
    {
        Value = ++valueCount;
        Name = name;
        if (!_lookup.TryAdd(valueCount, (T)this))
            throw new InvalidOperationException($"Duplicate value {valueCount} in {typeof(T).Name}");
    }
    
    public static T FromValue(int value)
    {
        if (_lookup.TryGetValue(value, out var result))
            return result;
        
        throw new ArgumentOutOfRangeException(nameof(value), $"Unknown {typeof(T).Name} value: {value}");
    }

    public static T FromName(string? name)
    {
        if (_lookup.FirstOrDefault(enumerable => enumerable.Value.Name == name).Value is { } result)
            return result;
        
        throw new ArgumentOutOfRangeException(nameof(name), $"Unknown {typeof(T).Name} name: {name}");
    }
    
    public override string ToString() => Name;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is T other && Value == other.Value;

    public static bool operator ==(StrongTypedEnumerable<T>? a, StrongTypedEnumerable<T>? b) => Equals(a, b);
    public static bool operator !=(StrongTypedEnumerable<T>? a, StrongTypedEnumerable<T>? b) => !Equals(a, b);
}