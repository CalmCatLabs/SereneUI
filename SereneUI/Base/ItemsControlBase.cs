using System;
using System.Collections.Generic;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

/// <summary>
/// Base for controls that can have multiple children.
/// </summary>
public abstract class ItemsControlBase : UiElementBase, IItemsControl
{
    /// <summary>
    /// Internal list of children.
    /// </summary>
    private readonly List<IUiElement> _children = [];

    /// <inheritdoc />
    public IReadOnlyList<IUiElement> Children => _children;
    
    /// <inheritdoc />
    public void AddChildren(IUiElement element)
    {
        if (element.Parent is not null) throw new InvalidOperationException("Element already has a parent.");
        
        element.Parent = this;
        _children.Add(element);
        
        InvalidateMeasure();
        InvalidateVisual();
    }

    /// <inheritdoc />
    public bool RemoveChildren(IUiElement element)
    {
        if (_children.Remove(element))
        {
            element.Parent = null;
            InvalidateMeasure();
            InvalidateVisual();
            return true;
        }
        return false;
    }
    
    /// <inheritdoc />
    public void ClearChildren()
    {
        _children.ForEach(i => i.Parent = null);
        _children.Clear();
        InvalidateMeasure();
        InvalidateVisual();
    }
}