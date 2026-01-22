using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serene.Common.Extensions;
using SereneUI.Base;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.EventArgs;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;
using HorizontalAlignment = SereneUI.Shared.Enums.HorizontalAlignment;
using Point = Microsoft.Xna.Framework.Point;
using VerticalAlignment = SereneUI.Shared.Enums.VerticalAlignment;

namespace SereneUI.ContentControls;

public class Page : ItemsControlBase
{
    private readonly Dictionary<IUiElement, int> _zIndex = [];
    private readonly Dictionary<IUiElement, Rectangle> _layoutBounds = []; // LOCAL bounds inside Page
    private readonly List<IUiElement> _focuableElements = [];
    private IUiElement? _currentFocusElement;

    public Page()
    {
        IsVisible = true;
        IsEnabled = true;

        Padding = new Thickness(0, 0, 0,0);
        Margin  = new Thickness(0, 0, 0,0);

        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment   = VerticalAlignment.Stretch;
    }

    // --- ZIndex ---
    public void SetZIndex(int index, IUiElement element) { _zIndex[element] = index; InvalidateVisual(); }
    public int GetZIndex(IUiElement element) => _zIndex.GetValueOrDefault(element, 0);
    
    public void BringToFront(IUiElement element)
    {
        int maxZ = _zIndex.Any() ? _zIndex.Max(p => p.Value) : 0;
        SetZIndex(maxZ + 1, element);
    }

    public bool TryGetChildBounds(IUiElement? element, out Rectangle localBounds)
    {
        localBounds = Rectangle.Empty;
        return element is not null && _layoutBounds.TryGetValue(element, out localBounds);
    }

    public UiElementBase? _dragElement;

    protected override void OnMeasure(in Point availableSize)
    {
        Size = availableSize;

        var clientSize = availableSize.Deflate(Padding);

        foreach (var child in Children)
        {
            if (!child.IsVisible) continue;

            // Child measures against page client area (minus its margin)
            var childAvailable = clientSize.Deflate(child.Margin);
            child.Measure(childAvailable);
        }
    }

    protected override void OnArrange(in Rectangle finalRect)
    {
        // client area inside padding
        var client = Bounds.Deflate(Padding);

        foreach (var child in Children)
        {
            if (!child.IsVisible) continue;

            if (TryGetChildBounds(child, out var local))
            {
                // If width/height are 0 -> fallback to measured size
                int w = local.Width  > 0 ? local.Width  : child.Size.X;
                int h = local.Height > 0 ? local.Height : child.Size.Y;

                // apply margin outside the child
                int x = child.PositionX ?? client.X + local.X + child.Margin.Left;
                int y = child.PositionY ?? client.Y + local.Y + child.Margin.Top;

                w = Math.Max(0, w - child.Margin.Horizontal);
                h = Math.Max(0, h - child.Margin.Vertical);

                child.Arrange(new Rectangle(x, y, w, h));
                continue;
            }

            // 2) Otherwise: Alignment-based layout inside the Page
            int innerLeft   = client.Left   + child.Margin.Left;
            int innerTop    = client.Top    + child.Margin.Top;
            int innerRight  = client.Right  - child.Margin.Right;
            int innerBottom = client.Bottom - child.Margin.Bottom;

            int innerW = Math.Max(0, innerRight - innerLeft);
            int innerH = Math.Max(0, innerBottom - innerTop);

            int w2 = child.Size.X;
            int h2 = child.Size.Y;

            int x2 = child.PositionX ?? innerLeft;
            int y2 = child.PositionY ?? innerTop;

            switch (child.HorizontalAlignment)
            {
                case HorizontalAlignment.Stretch: x2 = innerLeft; w2 = innerW; break;
                case HorizontalAlignment.Center:  x2 = innerLeft + (innerW - w2) / 2; break;
                case HorizontalAlignment.Right:   x2 = innerRight - w2; break;
            }

            switch (child.VerticalAlignment)
            {
                case VerticalAlignment.Stretch: y2 = innerTop; h2 = innerH; break;
                case VerticalAlignment.Center:  y2 = innerTop + (innerH - h2) / 2; break;
                case VerticalAlignment.Bottom:  y2 = innerBottom - h2; break;
            }

            child.Arrange(new Rectangle(x2, y2, Math.Max(0, w2), Math.Max(0, h2)));
        }
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        // Draw from back to front (low z -> high z)
        EnumerateChildrenForDraw().ForEach(child => child.Draw(spriteBatch));
    }

    protected override void OnUpdate(GameTime gameTime, UiInputData inputData)
    {
        base.OnUpdate(gameTime, inputData);
        var childHit = HitTestTopMostChild(inputData.MousePosition);

        if (_dragElement is not null)
        {
            _dragElement.HandleDragMove(inputData, inputData.MousePosition);
        }
        
        if (childHit is not null)
        {
            if (inputData.LeftMousePressed)
            {
                BringToFront(childHit);
            }
            childHit.HandleInput(inputData);
        }
    }

    private IEnumerable<IUiElement> EnumerateChildrenForDraw()
        => Children
            .Select((c, i) => (c, i, z: GetZIndex(c)))
            .OrderBy(t => t.z)
            .ThenBy(t => t.i)
            .Select(t => t.c);

    public IUiElement? HitTestTopMostChild(Point screenPoint)
        => Children
            .Select((c, i) => (c, i, z: GetZIndex(c)))
            .OrderByDescending(t => t.z)
            .ThenByDescending(t => t.i)
            .Select(t => t.c)
            .FirstOrDefault(c => c.HitTest(screenPoint));

    public void AddFocusableElement(IUiElement currentElement)
    {
        if (_focuableElements.Contains(currentElement)) return;
        _focuableElements.Add(currentElement);
        if (currentElement is UiElementBase ce)
        {
            ce.FocusEnter += OnFocusEnterHandler;
            ce.FocusLeave += OnFocusLeaveHandler;
        }
    }

    private void OnFocusLeaveHandler(object? sender, EventArgs e)
    {
        // if (sender is UiElementBase focusableElement)
        // {
        //     Debug.WriteLine($"{focusableElement.Id} lost focus.");
        //     //focusableElement.HasFocus = false;
        //     if (focusableElement.Equals(_currentFocusElement))
        //         _currentFocusElement = null;
        // }
    }

    private void OnFocusEnterHandler(object? sender, EventArgs e)
    {
        if (sender is UiElementBase focusableElement)
        {
            Debug.WriteLine($"{focusableElement.Id} got focus.");

            var childHasFocus = false;
            NodeUtility.ForAllNodesRun(focusableElement, node => childHasFocus = !childHasFocus && node.HasFocus);
            if (childHasFocus)
            {
                return;
            }
            
            focusableElement.HasFocus = true;
            
            if (_currentFocusElement is not null)
            {
                _currentFocusElement.HasFocus = false;
            }
            
            _focuableElements.ForEach(fe =>
            {
                if (fe == focusableElement) return;
                fe.HasFocus = false;
            });
            _currentFocusElement = focusableElement;
        }
        InvalidateMeasure();
        InvalidateVisual();
    }

    public void OnDragHandler(object? sender, UiMouseEventArgs e)
    {
        
    }

    public void OnDragEnterHandler(object? sender, UiMouseEventArgs e)
    {
        if (sender is UiElementBase dragElement)
        {
            _dragElement = dragElement;
        }
    }

    public void OnDragLeaveHandler(object? sender, UiMouseEventArgs e)
    {
        if (_dragElement is null) return;
        _dragElement = null;
    }
}
