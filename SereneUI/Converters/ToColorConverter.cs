using Microsoft.Xna.Framework;
using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(Color))]
public class ToColorConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = BuilderCommon.ParseColor(value);
        return true;
    }
}

[ConversionTargetType(typeof(Color?))]
public class ToNullableColorConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = BuilderCommon.ParseColor(value);
        return true;
    }
}