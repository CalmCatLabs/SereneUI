using Microsoft.Xna.Framework.Content;

namespace SereneUI.Interfaces;

public interface IConverter
{
    bool TryConvert(string value, out object? result);
}