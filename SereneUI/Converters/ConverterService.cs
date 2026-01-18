using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serene.Common.Extensions;
using SereneUI.Shared.Attributes;
using SereneUI.Interfaces;

namespace SereneUI.Converters;

public static class ConverterService
{
    private static Dictionary<Type, IConverter> Converters = [];
    
    public static void Initialize()
    {
        Assembly.GetAssembly(typeof(ConverterService))?
            .GetTypes()
            .Where(t => typeof(IConverter).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ForEach(converterType =>
            {
                var converterTypeAttribute = converterType.GetCustomAttribute<ConversionTargetTypeAttribute>();
                if (converterTypeAttribute is null) return;

                var instance = Activator.CreateInstance(converterType) as IConverter;
                if (instance is null) return;
                
                Converters.Add(converterTypeAttribute.Type, instance);
            });
    }

    public static object? Convert(Type targetType, string value)
    {
        if (Converters.TryGetValue(targetType, out var converter)
            && converter.TryConvert(value, out var result))
        {
            return result;
        }
        return null;
    }
}