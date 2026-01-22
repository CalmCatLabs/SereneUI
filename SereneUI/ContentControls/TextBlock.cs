using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Base;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;

namespace SereneUI.ContentControls;

public class TextBlock : UiElementBase
{
    private Vector2 _drawPosition = Vector2.Zero;
    public string? Text { get; set; } = string.Empty;
    public SpriteFont? Font { get; set; } = null!;
    public Color Color { get; set; } = Color.Black;

    public TextBlock(SpriteFont font)
    {
        Font = font;
        IsVisible = true;
        IsEnabled = true;

        Padding = new Thickness(0, 0, 0, 0);
        Margin = new Thickness(0, 0, 0, 0);
        
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;
    }
    
    public TextBlock()
    {
        IsVisible = true;
        IsEnabled = true;

        Padding = new Thickness(0, 0, 0, 0);
        Margin = new Thickness(0, 0, 0, 0);
        
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Top;
    }
    
    protected override void OnMeasure(in Point availableSize)
    {
        var measuredSize = Vector2.Zero;
        if (Font is not null)
            measuredSize = Font.MeasureString(Text ?? string.Empty);
        
        int textWidth = (int)MathF.Ceiling(measuredSize.X);
        int textHeight = (int)MathF.Ceiling(measuredSize.Y);
        
        int desiredWidth = textWidth + Padding.Horizontal;
        int desiredHeight = textHeight + Padding.Vertical;

        desiredWidth = Math.Min(desiredWidth, Math.Max(desiredWidth, availableSize.X));
        desiredHeight = Math.Min(desiredHeight, Math.Max(desiredHeight, availableSize.Y));
        
        Size = new Point(desiredWidth, desiredHeight);
    }

    protected override void OnArrange(in Rectangle finalRect)
    {
        var innerX = finalRect.X + Margin.Left + Padding.Left;
        var innerY = finalRect.Y + Margin.Top + Padding.Top;
        var innerWidth = Math.Max(0, finalRect.Width - (Padding.Left + Margin.Left + Padding.Right + Margin.Right));
        var innerHeight = Math.Max(0, finalRect.Height - (Padding.Top + Margin.Top + Padding.Bottom + Margin.Bottom));
        
        var measuredText = Vector2.Zero;
        if (Font is not null)
            measuredText = Font.MeasureString(Text ?? string.Empty);
        float textWidth = measuredText.X;
        float textHeight = measuredText.Y;
        
        float x = innerX;
        float y= innerY;
        
        // Horizontal
        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                x = innerX;
                break;
            case HorizontalAlignment.Center:
                x = innerX + (innerWidth - textWidth) * 0.5f;
                break;
            case HorizontalAlignment.Right:
                x = innerX + (innerWidth - textWidth);
                break;
            case HorizontalAlignment.Stretch:
                x = innerX + (innerWidth - textWidth) * 0.5f;
                break;
        }

        switch (VerticalAlignment)
        {
            case VerticalAlignment.Top:
                y = innerY;
                break;
            case VerticalAlignment.Center:
                y = innerY + (innerHeight - textHeight) * 0.5f;
                break;
            case VerticalAlignment.Bottom:
                y = innerY + (innerHeight - textHeight);
                break;
            case VerticalAlignment.Stretch:
                y = innerY + (innerHeight - textHeight) * 0.5f;
                break;
        }
        
        _drawPosition = new Vector2(MathF.Floor(x), MathF.Floor(y));
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, Text ?? string.Empty, _drawPosition, Color);
    }
}