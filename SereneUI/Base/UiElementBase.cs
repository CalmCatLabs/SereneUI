using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Converters;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.EventArgs;
using SereneUI.Shared.Interfaces;
using HorizontalAlignment = SereneUI.Shared.Enums.HorizontalAlignment;
using Point = Microsoft.Xna.Framework.Point;
using VerticalAlignment = SereneUI.Shared.Enums.VerticalAlignment;

namespace SereneUI.Base;

public abstract class UiElementBase : IUiElement
{
    public string? Id { get; set; }
    
    public Dictionary<string, string> MarkupExpressions { get; set; } = [];
    public object? DataContext { get; set; }
    public string? Class { get; set; }
    
    public int? Width { get; set; }
    public int? Height { get; set; }
    
    public Rectangle Bounds { get; set; }
    public Point Size { get; protected set; }
    
    public Thickness Padding { get; set; }
    public Thickness Margin { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; }
    public VerticalAlignment VerticalAlignment { get; set; }

    public bool IsEnabled
    {
        get => field;
        set
        {
            field = value;
            if (field) ApplyStyle();
            else ApplyStyle(":disabled");
        }
    }

    public bool IsVisible { get; set; }
    public bool IsDraggable { get; set; } = false;
    public IUiElement? Parent { get; set; }
    
    protected bool IsMeasureDirty { get; private set; } = true;
    protected bool IsArrangeDirty { get; private set; } = true;
    protected bool IsVisualDirty { get; private set; } = true;

    public Stylesheet? Stylesheet { get; set; } = null;
    
    public ICommand? OnMouseEnter { get; set; }
    public ICommand? OnMouseLeave { get; set; }
    public ICommand? OnMouseOver { get; set; }
    public ICommand? OnMouseDown { get; set; }
    public ICommand? OnMouseUp { get; set; }
    
    public event EventHandler<UiMouseEventArgs> MouseEnter;
    public event EventHandler<UiMouseEventArgs> MouseLeave;
    public event EventHandler<UiMouseEventArgs> MouseOver;
    public event EventHandler<UiMouseEventArgs> MouseDown;
    public event EventHandler<UiMouseEventArgs> MouseUp;

    protected bool _isMouseOver = false;

    public void ApplyStyle(string pseudoClass = "")
    {
        if (Stylesheet is null) return;
        var possibleSelectors = GetValidSelectors(pseudoClass);
        possibleSelectors.ForEach(selector =>
        {
            var styleRule = Stylesheet.StyleRules.FirstOrDefault(sr => sr.SelectorText.Equals(selector));
            if (styleRule is null) return;

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = styleRule.Style.GetPropertyValue(property.Name);
                if (string.IsNullOrWhiteSpace(value)) continue;
                var converted = ConverterService.Convert(property.PropertyType, value);
                property.SetValue(this, converted);
            }
        });
    }

    private List<string> GetValidSelectors(string pseudoClass)
    {
        if (!IsEnabled) pseudoClass = ":disabled";
        
        List<string> newList = [];
        if (!string.IsNullOrEmpty(pseudoClass))
        {
            newList.Add(pseudoClass);
        }
        newList.Add(GetType().Name + pseudoClass);
        if (!string.IsNullOrEmpty(Id))
        {
            newList.Add($"#{Id}{pseudoClass}");
        }
        
        if (!string.IsNullOrEmpty(Class))
        {
            newList.Add($"{GetType().Name}.{Class}{pseudoClass}");
            newList.Add($".{Class}{pseudoClass}");
        }

        
        return newList;
    }
    
    public void Measure(in Point availableSize)
    {
        if (!IsMeasureDirty) return;
        IsMeasureDirty = false;

        // 🌿 Width/Height begrenzen die "availableSize" für das Kind
        var constrained = ConstrainAvailableSize(availableSize);

        OnMeasure(constrained);

        // 🌙 Und nach dem Messen wird die gemessene Size fixiert, falls Width/Height gesetzt sind
        Size = ApplyFixedSize(Size);
    }

    public void Arrange(in Rectangle finalRectangle)
    {
        // 🌿 Falls feste Größe: Rectangle wird auf die feste Größe “geklemmt”
        var constrainedFinal = ConstrainFinalRect(finalRectangle);

        bool boundsChanged = Bounds != constrainedFinal;
        Bounds = constrainedFinal;

        if (!IsArrangeDirty && !boundsChanged) return;

        IsArrangeDirty = false;
        OnArrange(constrainedFinal);
    }

    public void Update(GameTime gameTime, UiInputData inputData)
    {
        if (!IsVisible || !IsEnabled)
        {
            UpdateChildren(gameTime, inputData);
            return;
        }
        
        var mousePosition = inputData.MousePosition;
        bool hit = HitTest(mousePosition);
        
        // enter
        if (hit && !_isMouseOver)
        {
            _isMouseOver = true;
            RaiseMouseEnter(mousePosition, inputData);
        }
        
        // over
        if (hit && _isMouseOver)
        {
            RaiseMouseOver(mousePosition, inputData);
        }
        
        // leave
        if (!hit && _isMouseOver)
        {
            _isMouseOver = false;
            RaiseMouseLeave(mousePosition, inputData);
        }
        OnUpdate(gameTime, inputData);
        UpdateChildren(gameTime, inputData);
    }
    
    protected virtual void OnUpdate(GameTime gameTime, UiInputData inputData) { }
    private void RaiseMouseEnter(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseEnter?.Invoke(this, args);
        TryExecute(OnMouseEnter, args, this);
    }
    
    private void RaiseMouseOver(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseOver?.Invoke(this, args);
        TryExecute(OnMouseOver, args, this);
    }
    
    private void RaiseMouseLeave(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseLeave?.Invoke(this, args);
        TryExecute(OnMouseLeave, args, this);
    }
    
    protected static void TryExecute(ICommand? cmd, UiMouseEventArgs args, object? sender = null)
    {
        if (cmd is null) return;
        if (cmd.CanExecute(sender: sender, args: args))
            cmd.Execute(sender: sender, args: args);
    }

    private void UpdateChildren(GameTime gameTime, UiInputData inputData)
    {
        if (this is ContentControlBase cc && cc.Content is UiElementBase childContent)
            childContent.Update(gameTime, inputData);

        if (this is ItemsControlBase items)
        {
            foreach (var child in items.Children)
                if (child is UiElementBase childBase)
                    childBase.Update(gameTime, inputData);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;
        OnDraw(spriteBatch);
        IsVisualDirty = false;
    }

    public void InvalidateMeasure()
    {
        IsMeasureDirty = true;
        IsArrangeDirty = true;
        Parent?.InvalidateMeasure();
    }

    public void InvalidateArrange()
    {
        IsArrangeDirty = true;
        Parent?.InvalidateArrange();
    }

    public void InvalidateVisual()
    {
        IsVisualDirty = true;
        Parent?.InvalidateVisual();
    }

    public bool HitTest(Point screenPoint)
    {
        return IsVisible && Bounds.Contains(screenPoint);
    }
    
    private Point ConstrainAvailableSize(in Point availableSize)
    {
        int w = availableSize.X;
        int h = availableSize.Y;

        if (Width.HasValue)  w = System.Math.Min(w, Width.Value);
        if (Height.HasValue) h = System.Math.Min(h, Height.Value);

        // keine negativen Werte zulassen
        if (w < 0) w = 0;
        if (h < 0) h = 0;

        return new Point(w, h);
    }

    private Point ApplyFixedSize(in Point measured)
    {
        int w = Width  ?? measured.X;
        int h = Height ?? measured.Y;

        if (w < 0) w = 0;
        if (h < 0) h = 0;

        return new Point(w, h);
    }

    private Rectangle ConstrainFinalRect(in Rectangle rect)
    {
        int w = Width  ?? rect.Width;
        int h = Height ?? rect.Height;

        if (w < 0) w = 0;
        if (h < 0) h = 0;

        // Position bleibt, Größe wird festgezurrt
        return new Rectangle(rect.X, rect.Y, w, h);
    }
    
    protected abstract void OnMeasure(in Point availableSize);
    protected abstract void OnArrange(in Rectangle finalRect);
    protected abstract void OnDraw(SpriteBatch spriteBatch);
}