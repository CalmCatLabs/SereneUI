using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Base;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Shared.EventArgs;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.ContentControls;

/// <summary>
/// Button element.
/// </summary>
public class Button : Panel
{
    private Texture2D? _pixel;
    private bool _wasMousePressed;

    /// <summary>
    /// Inner button text.
    /// </summary>
    public string? Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Inner text font.
    /// </summary>
    public SpriteFont Font { get; set; } = null!;
    
    /// <summary>
    /// Font color
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// On click command, gets fired when mouse gets up.
    /// </summary>
    public ICommand? OnClick { get; set; } = null;
    
    
    /// <summary>
    /// On click event, gets fired when mouse gets up.
    /// </summary>
    public event EventHandler<UiMouseEventArgs> Click = null!;
    
    /// <summary>
    /// Standard constructor.
    /// </summary>
    public Button()
    {
        IsVisible = true;
        IsEnabled = true;

        Padding = new Thickness(0, 0, 0, 0);
        Margin  = new Thickness(0, 0, 0, 0);

        // Diese Alignments gelten für *Panel selbst* (vom Parent zu beachten),
        // nicht zur Positionierung des Content.
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment   = VerticalAlignment.Top;
    }

    
    /// <inheritdoc />
    protected override void OnMeasure(in Point availableSize)
    {
        var innerAvailable = availableSize.Deflate(Padding);

        if (Content is null) return;

        // Kind bekommt Platz: innerAvailable minus Kind-Margin
        var childAvailable = innerAvailable.Deflate(Content.Margin);
        Content.Measure(childAvailable);

        // Panel wünscht: Kindgröße + Kind-Margin + eigenes Padding
        int desiredW = Content.Size.X + Content.Margin.Horizontal + Padding.Horizontal;
        int desiredH = Content.Size.Y + Content.Margin.Vertical + Padding.Vertical;

        desiredW = Math.Min(desiredW, Math.Max(0, availableSize.X));
        desiredH = Math.Min(desiredH, Math.Max(0, availableSize.Y));

        Size = new Point(desiredW, desiredH);
    }

    /// <inheritdoc />
    protected override void OnArrange(in Rectangle finalRect)
    {
        if (Content is null) return;
        
        // Innenraum des Panels
        var innerRect = finalRect.Deflate(Padding);

        // Slot für das Kind: Innenraum minus Kind-Margin (Margin ist außen ums Kind)
        var slot = innerRect.Deflate(Content.Margin);

        // Kindgröße (gemessen)
        int cw = Content.Size.X;
        int ch = Content.Size.Y;

        int x = slot.X;
        int y = slot.Y;

        // Alignment des KINDES innerhalb seines Slots
        switch (Content.HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                x = slot.X;
                break;

            case HorizontalAlignment.Center:
                x = slot.X + (slot.Width - cw) / 2;
                break;

            case HorizontalAlignment.Right:
                x = slot.Right - cw;
                break;

            case HorizontalAlignment.Stretch:
                // Stretch: wir geben volle Breite, Kind entscheidet, ob es sie nutzt
                x = slot.X;
                cw = Math.Max(0, slot.Width);
                break;
        }

        switch (Content.VerticalAlignment)
        {
            case VerticalAlignment.Top:
                y = slot.Y;
                break;

            case VerticalAlignment.Center:
                y = slot.Y + (slot.Height - ch) / 2;
                break;

            case VerticalAlignment.Bottom:
                y = slot.Bottom - ch;
                break;

            case VerticalAlignment.Stretch:
                y = slot.Y;
                ch = Math.Max(0, slot.Height);
                break;
        }

        Content.Arrange(new Rectangle(x, y, Math.Max(0, cw), Math.Max(0, ch)));
    }

    /// <inheritdoc />
    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;

        // 1x1 Pixel-Textur cachen
        if (_pixel is null || _pixel.GraphicsDevice != spriteBatch.GraphicsDevice)
        {
            _pixel?.Dispose();
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        spriteBatch.Draw(_pixel, Bounds, BackgroundColor);
        Content?.Draw(spriteBatch);
    }
    
    /// <inheritdoc />
    protected override void OnUpdate(GameTime gameTime, UiInputData inputData)
    {
        base.OnUpdate(gameTime, inputData);
        if (!IsEnabled) return;
        if (_isMouseOver && !inputData.LeftMousePressed)
        {
            ApplyStyle(":hover");
        }
        else if (_isMouseOver && inputData.LeftMousePressed)
        {
            ApplyStyle(":active");
        }
        else
        {
            ApplyStyle();
        }
        
        if (_isMouseOver && inputData.LeftMouseReleased)
        {
            RaiseClick(inputData.MousePosition, inputData);
            _wasMousePressed = false;
        }

        // if (!_isMouseOver && _wasMousePressed)
        // {
        //     _wasMousePressed = false;
        // }
        //
        // if (_isMouseOver && inputData.LeftMousePressed) _wasMousePressed = true;
    }
    
    private void RaiseClick(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        Click?.Invoke(this, args);
        TryExecute(OnClick, args, this);
    }
}