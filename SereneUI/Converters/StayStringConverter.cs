using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(string))]
public class ToStringConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = value;
        return true;
    }
}