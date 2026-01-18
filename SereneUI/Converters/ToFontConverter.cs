using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Converters;

[ConversionTargetType(typeof(SpriteFont))]
public class ToFontConverter : IConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        try
        {
            result = SereneUiSystem.Game.Content.Load<SpriteFont>(value);
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
        return false;
    }
}