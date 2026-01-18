using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExCSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Content;
using Serene.Common.Extensions;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Interfaces;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Builders;

public class BuildService(IServiceProvider serviceProvider)
{
    private readonly Dictionary<string, IUiElementBuilder> _uiElementBuilders = [];

    public void Initialize(params Assembly[]? assemblies)
    {
        _uiElementBuilders.Clear();
        if (assemblies == null || assemblies.Length < 1)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }
        
        assemblies.SelectMany(a => a.GetTypes())
            .Where(t => typeof(IUiElementBuilder).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .ForEach(builderType =>
            {
                var builderTargetType = builderType.GetCustomAttribute<BuilderTargetTypeAttribute>();
                if (builderTargetType is null) throw new NullReferenceException($"{nameof(builderTargetType)} was null for {nameof(builderType)}.");
                var instance = serviceProvider.GetRequiredService(builderType) as IUiElementBuilder;
                if (instance is null) return;
                _uiElementBuilders.Add(builderTargetType.Type.Name, instance);
            });
    }

    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var builder = _uiElementBuilders[node.Type];
        return builder.CreateUiElement(content, node, stylesheet, viewModel);
    }
}