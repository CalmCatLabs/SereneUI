using System;
using SereneUI.Interfaces;
using SereneUI.Shared.Attributes;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(Int32?))]
public class ToInt32Converter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        if (!string.IsNullOrEmpty(value) && Int32.TryParse(value, out var b))
        {
            result = b;
            return true;
        }
        return false;
    }
}