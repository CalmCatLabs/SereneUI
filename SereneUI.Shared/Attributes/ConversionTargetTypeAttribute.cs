using System;

namespace SereneUI.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConversionTargetTypeAttribute : Attribute
{
    public Type Type { get; private set; }

    public ConversionTargetTypeAttribute(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class BuilderTargetTypeAttribute : Attribute
{
    public Type Type { get; private set; }

    public BuilderTargetTypeAttribute(Type type)
    {
        Type = type;
    }
}