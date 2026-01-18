using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;
using SereneUI.Shared.Enums;
using SereneUI.Utilities;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(HorizontalAlignment))]
public class ToHorizontalAlignmentConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        if (BuilderCommon.TryGetEnum<HorizontalAlignment>(value, out var tmp))
        {
            result = tmp;
            return true;
        }
        return false;
    }
}