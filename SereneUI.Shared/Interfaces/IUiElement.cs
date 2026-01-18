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

/// <summary>
/// Base interface for ui elements. 
/// </summary>
public interface IUiElement
{
    /// <summary>
    /// Identity of an element.
    /// ToDo: should generate one automatically when the user
    /// did not set one. 
    /// </summary>
    string? Id { get; set; }
    
    /// <summary>
    /// Dictionary of known markup expressions.
    /// </summary>
    Dictionary<string, string> MarkupExpressions { get; }
    
    /// <summary>
    /// Property for the viewmodel/controller.
    /// </summary>
    object? DataContext { get; set; }
    
    /// <summary>
    /// Default Property for the style class.
    /// ToDo: make multiple possible.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Indicates if an element is draggable.
    /// </summary>
    bool IsDraggable { get; set; }
    
    /// <summary>
    /// Fixed width of the element.
    /// </summary>
    int? Width { get; set; }
    
    /// <summary>
    /// Fixed height of the element.
    /// </summary>
    int? Height { get; set; }
    
    /// <summary>
    /// Fixed position x of the element.
    /// </summary>
    public int? PositionX { get; set; }
    
    /// <summary>
    /// Fixed position y of the element. 
    /// </summary>
    public int? PositionY { get; set; }

    /// <summary>
    /// Just convenience to check if the position is a fixed one.
    /// </summary>
    public bool HasFixedPosition => PositionX.HasValue || PositionY.HasValue;
    
    // Layout
    /// <summary>
    /// Element measured bounds, x,y,w,h.
    /// </summary>
    Rectangle Bounds { get; set; }
    
    /// <summary>
    /// Elements measured size.
    /// </summary>
    Point Size { get; }
    
    /// <summary>
    /// Padding to the inner content.
    /// </summary>
    Thickness Padding { get; set; }
    
    /// <summary>
    /// Margin ti the outer container.
    /// </summary>
    Thickness Margin { get; set; }
    
    /// <summary>
    /// The horizontal alignment on arranged element. 
    /// </summary>
    HorizontalAlignment HorizontalAlignment { get; set; }
    
    /// <summary>
    /// the vertical alignment on arranged element.
    /// </summary>
    VerticalAlignment VerticalAlignment { get; set; }
    
    /// <summary>
    /// Is element enabled? 
    /// </summary>
    bool IsEnabled { get; set; }
    
    /// <summary>
    /// Is element visible?
    /// </summary>
    bool IsVisible { get; set; }
    
    /// <summary>
    /// Parent IUiElement if any (page does not have one.)
    /// </summary>
    IUiElement? Parent { get; set; }

    /// <summary>
    /// Method to apply a style.
    /// </summary>
    /// <param name="pseudoClass">optional, to give pseudoclasses like :hover, :drag, ...</param>
    public void ApplyStyle(string pseudoClass = "");
    
    /// <summary>
    /// Measure the sizes in the available space. 
    /// </summary>
    /// <param name="availableSize">X is width and Y = height.</param>
    void Measure(in Point availableSize);
    
    /// <summary>
    /// Arrange in the given rectangle.
    /// </summary>
    /// <param name="finalRectangle">Position and Size.</param>
    void Arrange(in Rectangle finalRectangle);
    
    /// <summary>
    /// Updates control.
    /// </summary>
    /// <param name="gameTime">The monogames game time object.</param>
    /// <param name="inputData">The input states.</param>
    void Update(GameTime gameTime, UiInputData inputData);
    
    /// <summary>
    /// Draw the control. 
    /// </summary>
    /// <param name="spriteBatch">The monogame SpriteBatch for drawing.</param>
    void Draw(SpriteBatch spriteBatch);
    
    /// <summary>
    /// Invalidates measures. 
    /// </summary>
    void InvalidateMeasure();
    /// <summary>
    /// Invalidates arrangement.
    /// </summary>
    void InvalidateArrange();
    /// <summary>
    /// Invalidates visual appearance.
    /// </summary>
    void InvalidateVisual();
    
    /// <summary>
    /// Check if the given point hits the control.
    /// </summary>
    /// <param name="screenPoint">Position on the screen.</param>
    /// <returns>true when hit, otherwise false.</returns>
    bool HitTest(Point screenPoint);
}