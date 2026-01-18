using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Serene.Common.Extensions;
using Serene.Common.Helpers;
using SereneUI.Converters;
using SereneUI.Interfaces;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Utilities;

public static class BuilderCommon
{
    public static void SetCommonAttributes(IUiElement element, UiNode node)
    {
        element.GetType().GetProperties()
            .ForEach(property =>
            {
                if (property.PropertyType.IsEnum)
                {
                    if (TryGetEnum(property.PropertyType, node, property.Name, out var enumValue) && enumValue is not null)
                        SetProp(element, nameof(property.Name), enumValue);
                }
                
                if (node.Attributes.TryGetValue(property.Name, out var attributeValue)) 
                {
                    
                    if (property.PropertyType == typeof(string))
                    {
                        SetProp(element, property.Name, attributeValue);
                    }
                    else if (ConverterService.TryConvert(property.PropertyType, attributeValue, out var convertedValue) && convertedValue is not null)
                    {
                        SetProp(element, property.Name, convertedValue);
                    }
                }
            });
        
        if (string.IsNullOrWhiteSpace(element.Id))
        {
            element.Id = RandomIdGenerator.Get(prefix: "ui-");
        }
    }
    
    public static Thickness ParseThickness(string s)
    {
        // "10" oder "10,20,10,20"
        var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
        {
            int v = int.Parse(parts[0]);
            return new Thickness(v, v, v, v);
        }
        if (parts.Length == 4)
        {
            return new Thickness(
                int.Parse(parts[0]), int.Parse(parts[1]),
                int.Parse(parts[2]), int.Parse(parts[3]));
        }
        throw new ContentLoadException($"Invalid Thickness: {s}");
    }

    public static Microsoft.Xna.Framework.Color ParseColor(string s)
    {
        // minimal: "Red" / "Teal" oder "#RRGGBB" oder "#RRGGBBAA"
        if (s.StartsWith("#"))
        {
            var hex = s.Substring(1);
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            if (hex.Length == 8)
            {
                byte a = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Microsoft.Xna.Framework.Color(r, g, b, a);
            }
            return new Microsoft.Xna.Framework.Color(r, g, b);
        }

        if (s.StartsWith("rgb("))
        {
            s = s.Replace("rgb(", "").Replace(")", "");
            var values = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (values.Length >= 3)
            {
                var r = byte.Parse(values[0]);
                var g = byte.Parse(values[1]);
                var b = byte.Parse(values[2]);
                if (values.Length == 4)
                {
                    var a = byte.Parse(values[3]);
                    return new Microsoft.Xna.Framework.Color(r, g, b, a);
                }
                return new Microsoft.Xna.Framework.Color(r, g, b);
            }
            
        }

        // Named colors via reflection (Color.Red etc.)
        var prop = typeof(Microsoft.Xna.Framework.Color).GetProperty(s);
        if (prop?.GetValue(null) is Microsoft.Xna.Framework.Color c) return c;

        throw new ContentLoadException($"Unknown Color: {s}");
    }

    public static void SetProp(object target, string propName, object value)
    {
        var p = target.GetType().GetProperty(propName);
        p?.SetValue(target, value);
    }

    public static bool TryGetEnum(Type enumType, UiNode node, string key, out object? value)
    {
        value = default;
        if (!node.Attributes.TryGetValue(key, out var s)) return false;
        if (s.Contains('.')) s = s.Split('.').Last(); // falls Processor nicht normalisiert
        return Enum.TryParse(enumType, s, out value);
    }
    
    public static bool TryGetEnum<TEnum>(UiNode node, string key, out TEnum value)
        where TEnum : struct
    {
        value = default;
        if (!node.Attributes.TryGetValue(key, out var s)) return false;
        if (s.Contains('.')) s = s.Split('.').Last(); // falls Processor nicht normalisiert
        return Enum.TryParse(s, out value);
    }
    
    public static bool TryGetEnum<TEnum>(string key, out TEnum value)
        where TEnum : struct
    {
        if (Enum.TryParse(key, ignoreCase: true, out value))
        {
            return true;
        }
        return false;
    }
}