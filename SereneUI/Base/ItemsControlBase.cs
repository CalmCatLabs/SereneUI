using System;
using System.Collections.Generic;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

public abstract class ItemsControlBase : UiElementBase, IItemsControl
{
    private readonly List<IUiElement> _items = [];

    public IReadOnlyList<IUiElement> Children => _items;
    
    public void AddItem(IUiElement element)
    {
        if (element.Parent is not null) throw new InvalidOperationException("Element already has a parent.");
        
        element.Parent = this;
        _items.Add(element);
        
        InvalidateMeasure();
        InvalidateVisual();
    }

    public bool RemoveItem(IUiElement element)
    {
        if (_items.Remove(element))
        {
            element.Parent = null;
            InvalidateMeasure();
            InvalidateVisual();
            return true;
        }
        return false;
    }

    public void ClearItems()
    {
        _items.ForEach(i => i.Parent = null);
        _items.Clear();
        InvalidateMeasure();
        InvalidateVisual();
    }
}