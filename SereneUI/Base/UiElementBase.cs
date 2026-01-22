using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using ExCSS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SereneUI.Converters;
using SereneUI.Extensions;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.EventArgs;
using SereneUI.Shared.Interfaces;
using Color = Microsoft.Xna.Framework.Color;
using HorizontalAlignment = SereneUI.Shared.Enums.HorizontalAlignment;
using Point = Microsoft.Xna.Framework.Point;
using VerticalAlignment = SereneUI.Shared.Enums.VerticalAlignment;

namespace SereneUI.Base;

/// <summary>
/// Base implementation for every ui element.
/// </summary>
public abstract class UiElementBase : ObservableObject, IUiElement
{
    /// <inheritdoc />
    public string? Id { get; set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public Dictionary<string, string> MarkupExpressions { get; init; } = [];
    
    /// <inheritdoc />
    public object? DataContext { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public string? Class { get; set => SetProperty(ref field, value); }

    public string? PseudoClass => string.Join(' ', appliedPseudoClasses);

    /// <inheritdoc />
    public int? Width { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public int? Height { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public int? PositionX { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public int? PositionY { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public bool HasFixedPosition => PositionX.HasValue || PositionY.HasValue;
    
    /// <inheritdoc />
    public Rectangle Bounds { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public Point Size { get; protected set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public Thickness Padding { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public Thickness Margin { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public Thickness? BorderThickness { get; set; } = Thickness.Zero;
    
    /// <inheritdoc />
    public Color? BorderColor { get; set; } = Color.Black;
    
    /// <inheritdoc />
    public HorizontalAlignment HorizontalAlignment { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public VerticalAlignment VerticalAlignment { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public bool IsEnabled
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            if (!field) AddPseudoClass("disabled"); 
            else RemovePseudoClass("disabled");
            ApplyStyle();
        }
    }
    
    /// <inheritdoc />
    public bool IsVisible { get; set => SetProperty(ref field, value); }
    
    /// <inheritdoc />
    public bool IsDraggable { get; set => SetProperty(ref field, value); } = false;


    /// <inheritdoc />
    public IUiElement? Parent { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Indicates if the measurement is dirty.
    /// </summary>
    protected bool IsMeasureDirty { get; private set => SetProperty(ref field, value); } = true;
    
    /// <summary>
    /// Indicates if the arrangement is dirty.
    /// </summary>
    protected bool IsArrangeDirty { get; private set => SetProperty(ref field, value); } = true;
    
    /// <summary>
    /// Indicates if the visual is dirty.
    /// </summary>
    protected bool IsVisualDirty { get; private set => SetProperty(ref field, value); } = true;

    /// <summary>
    /// Indicates if an element can be focused. 
    /// </summary>
    public bool IsFocusable { get; set => SetProperty(ref field, value); } = false;

    /// <summary>
    /// Indicates if the current element has focus. 
    /// </summary>
    public bool HasFocus { get; set => SetProperty(ref field, value, CheckFocusChange); } = false;

    private void CheckFocusChange()
    {
        if (!HasFocus)
        {
            RemovePseudoClass("focus");
            RaiseFocusLeave();
        }
        else if (HasFocus && _isMouseOver && appliedPseudoClasses.Contains("focus"))
        {
            RemovePseudoClass("focus");
        }
        else if (HasFocus && !_isMouseOver && !appliedPseudoClasses.Contains("focus"))
        {
            AddPseudoClass("focus");
        }
        
        InvalidateMeasure();
        InvalidateVisual();
        ApplyStyle();
    }

    /// <summary>
    /// ExCSS Stylesheet
    /// </summary>
    public Stylesheet? Stylesheet { get; set => SetProperty(ref field, value); } = null;
    
    /// <summary>
    /// Command to call when the mouse enters the element.
    /// </summary>
    public ICommand? OnMouseEnter { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when the mouse leaves the element.
    /// </summary>
    public ICommand? OnMouseLeave { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when the mouose is over the element.
    /// </summary>
    public ICommand? OnMouseOver { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when mouse gets down on element.
    /// </summary>
    public ICommand? OnMouseDown { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when mouse gets up on element
    /// </summary>
    public ICommand? OnMouseUp { get; set => SetProperty(ref field, value); }
    /// <summary>
    /// Command to call when dragging starts.
    /// </summary>
    public ICommand? OnDragEnter { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when the dragging ends.
    /// </summary>
    public ICommand? OnDragLeave { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call while dragging
    /// </summary>
    public ICommand? OnDrag { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when the element gets focused.
    /// </summary>
    public ICommand? OnFocusEnter { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Command to call when the element lost focus. 
    /// </summary>
    public ICommand? OnFocusLeave { get; set => SetProperty(ref field, value); }
    
    /// <summary>
    /// Event to fire when mouse enters the element. 
    /// </summary>
    public event EventHandler<UiMouseEventArgs> MouseEnter;
    
    /// <summary>
    /// Event to fire when mouse leaves the element.
    /// </summary>
    public event EventHandler<UiMouseEventArgs> MouseLeave;
    
    /// <summary>
    /// Event to fire when mouse is over the element.
    /// </summary>
    public event EventHandler<UiMouseEventArgs> MouseOver;
    
    /// <summary>
    /// Event to fire when mouse button goes down.
    /// </summary>
    public event EventHandler<UiMouseEventArgs> MouseDown;
    
    /// <summary>
    /// Event to fire when mouse button goes up.
    /// </summary>
    public event EventHandler<UiMouseEventArgs> MouseUp;
    
    /// <summary>
    /// Event to fire when drag starts
    /// </summary>
    public event EventHandler<UiMouseEventArgs> DragEnter;
    
    /// <summary>
    /// Event to fire when drag leaves
    /// </summary>
    public event EventHandler<UiMouseEventArgs> DragLeave;
    
    /// <summary>
    /// Event to fire when dragging
    /// </summary>
    public event EventHandler<UiMouseEventArgs> Drag;
    
    public event EventHandler FocusEnter;
    public event EventHandler FocusLeave;

    /// <summary>
    /// indicates if mouse is over the element.
    /// </summary>
    protected bool _isMouseOver = false;
    
    /// <summary>
    /// indicates if the mouse button is pressed.
    /// </summary>
    protected bool _isMouseDown = false;
    
    /// <summary>
    /// indicates if the element is in dragging mode.
    /// </summary>
    protected bool _isDragging;
    
    /// <summary>
    /// The offset between top/left of an element and the mouse position on it.
    /// </summary>
    private Point _dragOffsetInElement;

    private int _height;
    private int _width;

    private readonly List<string> appliedPseudoClasses = [];

    /// <inheritdoc />
    public void ApplyStyle(string pseudoClass = "")
    {
        if (Stylesheet is null) return;
        this.CompileObjectSelectors();
        var rules = this.MatchingRules(Stylesheet);
        
        rules.ToList().ForEach(rule =>
        {
            if (rule is null) return;
            
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = rule.Style.GetPropertyValue(property.Name);
                if (string.IsNullOrWhiteSpace(value)) continue;
                var converted = ConverterService.Convert(property.PropertyType, value);
                property.SetValue(this, converted);
            }
        });
    }

    public void AddPseudoClass(string pseudoClass)
    {
        if (appliedPseudoClasses.Contains(pseudoClass)) return;
        appliedPseudoClasses.Add(pseudoClass);
    }
    
    public void RemovePseudoClass(string pseudoClass)
    {
        if (appliedPseudoClasses.Contains(pseudoClass))
            appliedPseudoClasses.Remove(pseudoClass);
    }

    /// <summary>
    /// Returns valid selectors for this element, to search for in the stylesheet.
    /// </summary>
    /// <param name="pseudoClass"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Measure method for the element.
    /// Only runs when IsMeasureDirty is true.
    /// </summary>
    /// <param name="availableSize">In parent available size.</param>
    public void Measure(in Point availableSize)
    {
        if (!IsMeasureDirty) return;
        IsMeasureDirty = false;
        var constrained = ConstrainAvailableSize(availableSize);

        OnMeasure(constrained);
        
        Size = ApplyFixedSize(Size);
    }

    /// <summary>
    /// Arrange the element on  specific position.
    /// </summary>
    /// <param name="finalRectangle"></param>
    public void Arrange(in Rectangle finalRectangle)
    {
        var constrainedFinal = ConstrainFinalRect(finalRectangle);

        bool boundsChanged = Bounds != constrainedFinal;
        Bounds = constrainedFinal;

        if (!IsArrangeDirty && !boundsChanged) return;

        IsArrangeDirty = false;
        OnArrange(constrainedFinal);
    }

    /// <summary>
    /// Handles input and fires events if the requirement is met.
    /// </summary>
    /// <param name="gameTime">Monogame GameTime object.</param>
    /// <param name="inputData">Input states.</param>
    public void Update(GameTime gameTime, UiInputData inputData)
    {
        if (!IsVisible || !IsEnabled)
        {
            UpdateChildren(gameTime, inputData);
            return;
        }
        
        OnUpdate(gameTime, inputData);
        UpdateChildren(gameTime, inputData);
    }
    
    /// <summary>
    /// Handles the drag and drop stuff.
    /// </summary>
    /// <param name="input">Input states</param>
    /// <param name="mousePosition">Current position of the mouse.</param>
    /// <param name="hit">Are we over the element?</param>
    public void HandleDrag(UiInputData input, Point mousePosition, bool hit)
    {
        if (IsDraggable && input.LeftMouseDown && hit)
        {
            _isDragging = true;

            // Element absolute pos im selben Raum wie mouseInScreenOrUi:
            var elementAbs = new Point(Bounds.X, Bounds.Y);

            // Offset: wo greifst du das Element an?
            _dragOffsetInElement = mousePosition - elementAbs;
            AddPseudoClass("drag");
            ApplyStyle();
            InvalidateMeasure();
            InvalidateVisual();
            RaiseDragEnter(mousePosition, input);
        }
    }

    public void HandleDragMove(UiInputData input, Point mousePosition)
    {
        if (_isDragging && input.LeftMousePressed)
        {
            RaiseDrag(mousePosition, input);
            // Ziel: Element so bewegen, dass der anfängliche Griffpunkt unter der Maus bleibt
            var newAbs = mousePosition - _dragOffsetInElement;

            PositionX = Math.Max(0 - (int)Bounds.Width / 2, newAbs.X); 
            PositionY = Math.Max(0 - (int)Bounds.Height / 2, newAbs.Y);
            PositionX = Math.Min(_width - (int)Bounds.Width / 2, PositionX.Value); 
            PositionY = Math.Min(_height - (int)Bounds.Height / 2, PositionY.Value); 
            InvalidateArrange();
        }
        else if (_isDragging && input.LeftMouseReleased)
        {
            _isDragging = false;
            RemovePseudoClass("drag");
            InvalidateMeasure();
            InvalidateVisual();
            ApplyStyle();
            RaiseDragLeave(mousePosition, input);
        }
    }
    
    /// <summary>
    /// Overwriteable method where custom stuff can be done. 
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="inputData"></param>
    protected virtual void OnUpdate(GameTime gameTime, UiInputData inputData) { }
    
    /// <summary>
    /// Raises the event and command for mouse up.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    private void RaiseMouseUp(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseUp?.Invoke(this, args);
        TryExecute(OnMouseUp, args, this);
    }
    
    /// <summary>
    /// Raises the event and command for mouse down.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    private void RaiseMouseDown(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseDown?.Invoke(this, args);
        TryExecute(OnMouseDown, args, this);
    }
    
    /// <summary>
    /// Raises the event and command for mouse enter.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    private void RaiseMouseEnter(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseEnter?.Invoke(this, args);
        TryExecute(OnMouseEnter, args, this);
    }
    
    /// <summary>
    /// Raises the event and command for mouse over.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    private void RaiseMouseOver(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        var args = new UiMouseEventArgs(pos, input);
        MouseOver?.Invoke(this, args);
        TryExecute(OnMouseOver, args, this);
    }
    
    /// <summary>
    /// Raises the event and command for mouse leave.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    private void RaiseMouseLeave(Microsoft.Xna.Framework.Point pos, UiInputData input)
    {
        _isMouseOver = false;
        _isMouseDown = false;
        
        var args = new UiMouseEventArgs(pos, input);
        MouseLeave?.Invoke(this, args);
        TryExecute(OnMouseLeave, args, this);
    }
    
    /// <summary>
    /// Tries to execute the command, if any and can execute returns true.
    /// </summary>
    /// <param name="pos">Mouse position</param>
    /// <param name="input">Input states.</param>
    protected static void TryExecute<TArgs>(ICommand? cmd, TArgs args, object? sender = null)
    {
        if (cmd is null) return;
        if (cmd.CanExecute(sender: sender, args: args))
            cmd.Execute(sender: sender, args: args);
    }
    
    /// <summary>
    /// Run updates on content or children,
    /// depending on ControlBase type.
    /// </summary>
    /// <param name="gameTime">Monogame GameTime object.</param>
    /// <param name="inputData">Input states.</param>
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

    /// <summary>
    /// Draw yourself on the spritebatch.
    /// </summary>
    /// <param name="spriteBatch">Monogame SpriteBatch.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;
        _height = spriteBatch.GraphicsDevice.Viewport.Height;
        _width = spriteBatch.GraphicsDevice.Viewport.Width;
        OnDraw(spriteBatch);
        IsVisualDirty = false;
    }

    /// <summary>
    /// Sets IsMeasureDirty = true and IsArrangeDirty = true
    /// And calls InvalidateMeasure on a parent if there is one.
    /// </summary>
    public void InvalidateMeasure()
    {
        IsMeasureDirty = true;
        IsArrangeDirty = true;
        Parent?.InvalidateMeasure();
    }
    
    /// <summary>
    /// Sets IsArrangeDirty = true
    /// And calls InvalidateArrange on a parent if there is one.
    /// </summary>
    public void InvalidateArrange()
    {
        IsArrangeDirty = true;
        Parent?.InvalidateArrange();
    }
    
    /// <summary>
    /// Sets IsVisualDirty = true
    /// And calls InvalidateVisual on a parent if there is one.
    /// </summary>
    public void InvalidateVisual()
    {
        IsVisualDirty = true;
        Parent?.InvalidateVisual();
    }

    /// <summary>
    /// Check if a given point is inside the elements bounds.
    /// </summary>
    /// <param name="screenPoint">Point e.g. mouse position.</param>
    /// <returns>true if point is inside elements bounds, otherwise false</returns>
    public bool HitTest(Point screenPoint)
    {
        return IsVisible && Bounds.Contains(screenPoint);
    }
    
    /// <summary>
    /// Clamp to size if there is one. 
    /// </summary>
    /// <param name="availableSize">Maximum available measured size</param>
    /// <returns></returns>
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

    /// <summary>
    /// Applies a fixed size to an object.
    /// </summary>
    /// <param name="measured"></param>
    /// <returns></returns>
    private Point ApplyFixedSize(in Point measured)
    {
        int w = Width  ?? measured.X;
        int h = Height ?? measured.Y;

        if (w < 0) w = 0;
        if (h < 0) h = 0;

        return new Point(w, h);
    }

    /// <summary>
    /// constrains the element to a given rect if no fixes size is given.
    /// </summary>
    /// <param name="rect">Given position and size rectangle.</param>
    /// <returns>The resulting rectangle.</returns>
    private Rectangle ConstrainFinalRect(in Rectangle rect)
    {
        int w = Width  ?? rect.Width;
        int h = Height ?? rect.Height;

        if (w < 0) w = 0;
        if (h < 0) h = 0;

        // Position bleibt, Größe wird festgezurrt
        return new Rectangle(rect.X, rect.Y, w, h);
    }

    public virtual void HandleTextInput(TextInputEventArgs textInputEventArgs)
    {
    }

    public virtual void HandleInput(UiInputData inputData)
    {
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

        if (hit && !_isMouseDown && inputData.LeftMouseDown)
        {
            _isMouseDown = true;
            RaiseMouseDown(mousePosition, inputData);
        }
        
        if (hit && _isMouseDown && inputData.LeftMouseReleased)
        {
            _isMouseDown = false;
            RaiseMouseUp(mousePosition, inputData);
        }
        HandleFocus(inputData, mousePosition, hit);
        HandleDrag(inputData, mousePosition, hit);
    }

    private void HandleFocus(UiInputData inputData, Point mousePosition, bool hit)
    {
        if (!IsVisible) return;
        if (!IsEnabled) return;
        if (!IsFocusable) return;
        
        if (!HasFocus && hit && inputData.LeftMouseDown)
        {
            HasFocus = true;
            RaiseFocusEnter(mousePosition, inputData);
        }
    }

    private void RaiseFocusEnter(Point mousePosition, UiInputData inputData)
    {
        var args = new UiMouseEventArgs(mousePosition, inputData);
        FocusEnter?.Invoke(this, args);
        TryExecute(OnFocusEnter, args, this);
    }

    private void RaiseFocusLeave()
    {
        FocusLeave?.Invoke(this, EventArgs.Empty);
        TryExecute<object?>(OnFocusLeave, null, this);
    }
    
    private void RaiseDragEnter(Point mousePosition, UiInputData inputData)
    {
        var args = new UiMouseEventArgs(mousePosition, inputData);
        DragEnter?.Invoke(this, args);
        TryExecute(OnDragEnter, args, this);
    }
    
    private void RaiseDragLeave(Point mousePosition, UiInputData inputData)
    {
        var args = new UiMouseEventArgs(mousePosition, inputData);
        DragLeave?.Invoke(this, args);
        TryExecute(OnDragLeave, args, this);
    }
    
    private void RaiseDrag(Point mousePosition, UiInputData inputData)
    {
        var args = new UiMouseEventArgs(mousePosition, inputData);
        Drag?.Invoke(this, args);
        TryExecute(OnDrag, args, this);
    }

    /// <summary>
    /// Stub for child classes to hang into the measurement.
    /// </summary>
    /// <param name="availableSize">Maximum available space.</param>
    protected abstract void OnMeasure(in Point availableSize);
    
    /// <summary>
    /// Stub for child classes to hang into the arrangement.
    /// </summary>
    /// <param name="finalRect">Current position and size.</param>
    protected abstract void OnArrange(in Rectangle finalRect);
    
    /// <summary>
    /// Stub for child classes to hang into the drawing
    /// </summary>
    /// <param name="spriteBatch">Monogames SpriteBatch.</param>
    protected abstract void OnDraw(SpriteBatch spriteBatch);
}
