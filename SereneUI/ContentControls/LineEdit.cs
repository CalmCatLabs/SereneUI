using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SereneUI.Shared.DataStructures;

namespace SereneUI.ContentControls;

public class LineEdit : Panel
{
    public string Placeholder { get; set => SetProperty(ref field, value, SetInnerTextIfEmpty); } = string.Empty;

    private void SetInnerTextIfEmpty()
    {
        if (Content is TextBlock textBlock && string.IsNullOrEmpty(textBlock.Text))
        {
            textBlock.Text = Placeholder;
        }
    }

    public TextBlock? InnerTextBlock { get; set; }

    public SpriteFont Font
    {
        get;
        set => SetProperty(ref field, value, FontChangedHandler);
    } = null!;

    public Color? Color { get => InnerTextBlock?.Color; set => SetProperty(ref field, value, ColorChangedHandler); }

    public LineEdit()
    {
        IsFocusable = true;
    }
    
    public override void HandleInput(UiInputData inputData)
    {
        base.HandleInput(inputData);
        if (HasFocus && (PseudoClass is null || !PseudoClass.Contains("focus"))) AddPseudoClass("focus");
        if (HasFocus && Content is TextBlock textBlock && Placeholder.Equals(textBlock.Text))
        {
            textBlock.Text = string.Empty;
            textBlock.InvalidateMeasure();
            textBlock.InvalidateVisual();
        }
        if (!HasFocus && Content is TextBlock textBlock2 && string.IsNullOrWhiteSpace(textBlock2.Text))
        {
            textBlock2.Text = Placeholder;
            textBlock2.InvalidateMeasure();
            textBlock2.InvalidateVisual();
        }
        
        ApplyStyle();
        Content?.HandleInput(inputData);
    }
    
    public override void HandleTextInput(TextInputEventArgs textInputEventArgs)
    {
        if (!HasFocus)
            return;

        char c = textInputEventArgs.Character;

        if (Content is not TextBlock textBlock) return;
        textBlock.Font = Font;
        
        if (!char.IsControl(c) )
        {
            textBlock.Text += c.ToString();
        }
        else if (textInputEventArgs.Key == Keys.Back && !string.IsNullOrWhiteSpace(textBlock.Text))
        {
            textBlock.Text = textBlock.Text?.Substring(0, textBlock.Text.Length - 1);
        }
        else if (textInputEventArgs.Key == Keys.Enter)
        {
            textBlock.Text += Environment.NewLine;
        }
        textBlock.InvalidateMeasure();
        textBlock.InvalidateVisual();
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
        //content.ApplyStyle();
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