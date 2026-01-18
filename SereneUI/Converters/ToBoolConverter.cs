using SereneUI.Interfaces;
using SereneUI.Shared.Attributes;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(bool))]
public class ToBoolConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = false;
        if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out bool b))
        {
            result = b;
            return true;
        }
        return false;
    }
}