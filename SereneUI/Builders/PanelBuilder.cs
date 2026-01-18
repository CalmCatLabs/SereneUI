using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

[BuilderTargetType(typeof(Panel))]
public class PanelBuilder(BuildService buildService) : IUiElementBuilder
{
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var panel = new Panel()
        {
            Stylesheet = stylesheet,
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(panel, node);
        panel.ApplyStyle();
        
        if (node.Children.FirstOrDefault() is { } child 
            && buildService.CreateUiElement(content, child, stylesheet, viewModel) is IUiElement uiElement)
        {
            uiElement.Parent = panel;
            panel.Content = uiElement;
        }
        
        if (node.Attributes.TryGetValue(nameof(panel.BackgroundColor), out var backgroundColor))
        {
            panel.BackgroundColor = BuilderCommon.ParseColor(backgroundColor);
        }
        return panel;
    }
}