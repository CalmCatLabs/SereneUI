using Microsoft.Xna.Framework.Content;

namespace SereneUI.Interfaces;

/// <summary>
/// Interface for converters. 
/// </summary>
public interface IConverter
{
    /// <summary>
    /// Tries to convert a string value to the type the converter will produce. 
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="result">Resulting object on success.</param>
    /// <returns>true on success, otherwise false.</returns>
    bool TryConvert(string value, out object? result);
}