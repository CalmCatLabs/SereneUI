using System;

namespace SereneUI.Shared.Attributes;

/// <summary>
/// An attribute to help the CoverterService. saving
/// a converter implementation to a specific target type.
/// If this is missing the converter won't be registered.  
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ConversionTargetTypeAttribute : Attribute
{
    /// <summary>
    /// Target type
    /// </summary>
    public Type Type { get; private set; }

    /// <summary>
    /// Constructs the attribute with the target type.
    /// </summary>
    /// <param name="type">Converters target type.</param>
    public ConversionTargetTypeAttribute(Type type)
    {
        Type = type;
    }
}

/// <summary>
/// Attribute to help the BuildService to save a control builder to a specific controll type.
/// If its missing the builder won't be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BuilderTargetTypeAttribute : Attribute
{
    /// <summary>
    /// Target Type.
    /// </summary>
    public Type Type { get; private set; }

    /// <summary>
    /// Constructs the attribute with the target type.
    /// </summary>
    /// <param name="type">Builders target type.</param>
    public BuilderTargetTypeAttribute(Type type)
    {
        Type = type;
    }
}