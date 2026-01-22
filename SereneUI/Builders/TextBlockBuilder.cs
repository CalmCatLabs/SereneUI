using ExCSS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

/// <summary>
/// 
/// </summary>
[BuilderTargetType(typeof(TextBlock))]
public class TextBlockBuilder : IUiElementBuilder
{
    /// <inheritdoc />
    public object? CreateUiElement(Game game, ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var textBlock = new TextBlock
        {
            Text = node.InnerText,
            Stylesheet = stylesheet,
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(textBlock, node);
        textBlock.ApplyStyle();
        
        return textBlock;
    }
}