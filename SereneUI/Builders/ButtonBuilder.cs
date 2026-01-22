using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

/// <summary>
/// Class that creates a button. 
/// </summary>
/// <param name="buildService"></param>
[BuilderTargetType(typeof(Button))]
public class ButtonBuilder(BuildService buildService): IUiElementBuilder
{
    /// <inheritdoc />
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var button = new Button
        {
            Text = node.InnerText,
            Stylesheet = stylesheet,
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(button, node);
        button.ApplyStyle();
        
        if (node.Children.FirstOrDefault() is { } child 
            && buildService.CreateUiElement(content, child, stylesheet, viewModel) is IUiElement uiElement)
        {
            uiElement.Parent = button;
            button.Content = uiElement;
        }
        return button;
    }
}