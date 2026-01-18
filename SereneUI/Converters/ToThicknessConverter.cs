using Microsoft.Xna.Framework;
using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;
using SereneUI.Shared.DataStructures;
using SereneUI.Utilities;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(Thickness))]
public class ToThicknessConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = BuilderCommon.ParseThickness(value);
        return true;
    }
}