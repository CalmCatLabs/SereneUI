using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Shared.DataStructures;
using HorizontalAlignment = SereneUI.Shared.Enums.HorizontalAlignment;
using Point = Microsoft.Xna.Framework.Point;
using VerticalAlignment = SereneUI.Shared.Enums.VerticalAlignment;

// ReSharper disable once CheckNamespace
namespace SereneUI.Shared.Interfaces;

public interface IUiElement
{
    // Identity 
    string? Id { get; set; }
    Dictionary<string, string> MarkupExpressions { get; }
    object? DataContext { get; set; }
    string? Class { get; set; }

    bool IsDraggable { get; set; }
    int? Width { get; set; }
    int? Height { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }

    // Optional: Komfort
    public bool HasFixedPosition => PositionX.HasValue || PositionY.HasValue;
    
    // Layout
    Rectangle Bounds { get; set; }
    Point Size { get; }
    
    Thickness Padding { get; set; }
    Thickness Margin { get; set; }
    HorizontalAlignment HorizontalAlignment { get; set; }
    VerticalAlignment VerticalAlignment { get; set; }
    
    bool IsEnabled { get; set; }
    bool IsVisible { get; set; }
    
    // Ui Tree
    IUiElement? Parent { get; set; }

    public void ApplyStyle(string pseudoClass = "");
    
    // Lifecycle
    void Measure(in Point availableSize);
    void Arrange(in Rectangle finalRectangle);
    
    // Update + Draw
    void Update(GameTime gameTime, UiInputData inputData);
    void Draw(SpriteBatch spriteBatch);
    
    // Invalidation
    void InvalidateMeasure();
    void InvalidateArrange();
    void InvalidateVisual();
    bool HitTest(Point screenPoint);
}