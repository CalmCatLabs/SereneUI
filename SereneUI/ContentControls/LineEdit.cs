using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Shared.DataStructures;

namespace SereneUI.ContentControls;

public class LineEdit : Panel
{
    
    public TextBlock? InnerTextBlock { get; set; }
    public SpriteFont? Font { get => InnerTextBlock?.Font; set => SetProperty(ref field, value, FontChangedHandler); }

    public Color? Color { get => InnerTextBlock?.Color; set => SetProperty(ref field, value, ColorChangedHandler); }

    public LineEdit()
    {
        IsFocusable = true;
    }
    
    public override void HandleInput(UiInputData inputData)
    {
        base.HandleInput(inputData);
        if (HasFocus && (PseudoClass is null || !PseudoClass.Contains("focus"))) AddPseudoClass("focus");
        ApplyStyle();
        Content?.HandleInput(inputData);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        base.OnDraw(spriteBatch);
        if (Content is TextBlock textBlock)
        {
            textBlock.Draw(spriteBatch);
        }
    }

    private void FontChangedHandler()
    {
        if (Content is null) return;
        var content = Content as TextBlock;
        content.Font = Font;
        content.ApplyStyle();
    }
    private void ColorChangedHandler()
    {
        if (Color != null && Content != null)
        {
            var content = Content as TextBlock;
            content.Color = Color.Value;
            InnerTextBlock?.ApplyStyle();
        }
    }
}