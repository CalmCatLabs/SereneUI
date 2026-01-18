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
/// 
/// </summary>
[BuilderTargetType(typeof(TextBlock))]
public class TextBlockBuilder : IUiElementBuilder
{
    /// <inheritdoc />
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
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
        
        if (node.Attributes.TryGetValue(nameof(textBlock.Font), out var fontName))
        {
            textBlock.Font = content.Load<SpriteFont>(fontName);
        }
        
        if (node.Attributes.TryGetValue(nameof(textBlock.Color), out var color))
        {
            textBlock.Color = BuilderCommon.ParseColor(color);
        }
        
        return textBlock;
    }
}