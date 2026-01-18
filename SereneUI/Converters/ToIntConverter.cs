using System;
using SereneUI.Interfaces;
using SereneUI.Shared.Attributes;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(int))]
public class ToIntConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var b))
        {
            result = b;
            return true;
        }
        return false;
    }
}