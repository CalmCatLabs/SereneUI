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

    private Vector2 _cursorOffset = Vector2.Zero;
    private int _cursorIndex = 0;
    private Texture2D? _pixelCursor;
    private bool _cursorIsVisible = false;
    public int CursorBlinkSpeed
    {
        get => _cursorBlinkSpeed.Milliseconds;
        set => _cursorBlinkSpeed = new TimeSpan(0,0,0,0,value);
    }
    private TimeSpan _cursorBlinkSpeed = new TimeSpan(0,0,0,0,300);
    private TimeSpan? lastToggleTime = null;
    
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
            ++_cursorIndex;
        }
        else if (textInputEventArgs.Key == Keys.Back && !string.IsNullOrWhiteSpace(textBlock.Text))
        {
            textBlock.Text = textBlock.Text?.Substring(0, textBlock.Text.Length - 1);
            --_cursorIndex;
        }
        else if (textInputEventArgs.Key == Keys.Enter)
        {
            textBlock.Text += Environment.NewLine;
        }
        
        // Cursor
        if (!string.IsNullOrWhiteSpace(textBlock.Text) && textBlock.Text != Placeholder)
        {
            var textToCursor = textBlock.Text.Substring(0, _cursorIndex);
            _cursorOffset = textBlock.Font.MeasureString(textToCursor);
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
            
            // Draw Cursor
            if (HasFocus && _cursorIsVisible)
            {// 1x1 Pixel-Textur cachen
                if (_pixelCursor is null || _pixelCursor.GraphicsDevice != spriteBatch.GraphicsDevice)
                {
                    _pixelCursor?.Dispose();
                    _pixelCursor = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    _pixelCursor.SetData(new[] { Microsoft.Xna.Framework.Color.Black });
                }
                
                var cursorHeight = textBlock.Font?.MeasureString("|").Y ?? 20;
                var topPadding = Bounds.Y + (Bounds.Height-cursorHeight) / 2;

                string lastChar = textBlock.Text?.Length > 0 ? $"{textBlock.Text[^1]}" : "";
                var lastCharWidth = textBlock.Font?.MeasureString(lastChar).X;
                var positionX = Bounds.Left + (int)_cursorOffset.X + 7;
                var cursorBounds = new Rectangle((int)positionX, (int)topPadding, 2, (int)cursorHeight);
                
                spriteBatch.Draw(_pixelCursor, cursorBounds, BackgroundColor);
            }
        }
    }

    protected override void OnUpdate(GameTime gameTime, UiInputData inputData)
    {
        base.OnUpdate(gameTime, inputData);

        if (!HasFocus) return;
        if (lastToggleTime == null 
            || (lastToggleTime.HasValue && gameTime.TotalGameTime.Subtract(lastToggleTime.Value) > _cursorBlinkSpeed))
        {
            _cursorIsVisible = !_cursorIsVisible;
            lastToggleTime = gameTime.TotalGameTime;
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