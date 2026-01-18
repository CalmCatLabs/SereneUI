
using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

/// <summary>
/// Creates a stack panel.
/// </summary>
/// <param name="buildService">The build service.</param>
[BuilderTargetType(typeof(StackPanel))]
public class StackPanelBuilder(BuildService buildService): IUiElementBuilder
{
    /// <inheritdoc />
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var panel = new StackPanel
        {
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(panel, node);
        panel.ApplyStyle();

        if (BuilderCommon.TryGetEnum(node, nameof(Orientation), out Orientation orientation))
            BuilderCommon.SetProp(panel, nameof(panel.Orientation), orientation);
        
        node.Children.ForEach(child =>
        {
            if (buildService.CreateUiElement(content, child, stylesheet, viewModel) is IUiElement uiElement)
            {
                panel.AddChildren(uiElement);
            }
        });
        
        return panel;
    }
}