using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExCSS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Serene.Common.Extensions;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Interfaces;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Builders;

/// <summary>
/// Service to build up the ui using UiNodes filled by xml.
/// </summary>
/// <param name="serviceProvider">IServiceProvider.</param>
public class BuildService(IServiceProvider serviceProvider)
{
    private readonly Dictionary<string, IUiElementBuilder> _uiElementBuilders = [];

    /// <summary>
    /// Initializes the internal builder list.
    /// </summary>
    /// <exception cref="NullReferenceException">Throws when there is no builder target type.</exception>
    public void Initialize()
    {
        _uiElementBuilders.Clear();
        AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
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

    /// <summary>
    /// Create the elements from the ui nodes. 
    /// </summary>
    /// <param name="content">Monogames content manager.</param>
    /// <param name="node">Current ui node.</param>
    /// <param name="stylesheet">Stylesheet for the ui.</param>
    /// <param name="viewModel">optional view model.</param>
    /// <returns></returns>
    public object? CreateUiElement(Game game, ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var builder = _uiElementBuilders[node.TagName];
        return builder.CreateUiElement(game, content, node, stylesheet, viewModel);
    }
}