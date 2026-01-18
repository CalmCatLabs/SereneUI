using System;
using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;
using SereneUI.Shared.Enums;
using SereneUI.Utilities;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(VerticalAlignment))]
public class ToVerticalAlignmentConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        if (BuilderCommon.TryGetEnum<VerticalAlignment>(value, out var tmp))
        {
            result = tmp;
            return true;
        }
        return false;
    }
}