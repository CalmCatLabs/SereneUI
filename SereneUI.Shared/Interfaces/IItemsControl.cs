using System.Collections.Generic;
using SereneUI.Interfaces;

namespace SereneUI.Shared.Interfaces;

public interface IItemsControl
{
    IReadOnlyList<IUiElement> Children { get; }
    void AddItem(IUiElement element);
    bool RemoveItem(IUiElement element);
    void ClearItems();
}