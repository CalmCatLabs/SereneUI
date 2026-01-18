using System.Collections.Generic;
using SereneUI.Interfaces;

namespace SereneUI.Shared.Interfaces;

/// <summary>
/// Interface for a control that can hold multiple items.  
/// </summary>
public interface IItemsControl
{
    /// <summary>
    /// List of child IUiElement's.
    /// </summary>
    IReadOnlyList<IUiElement> Children { get; }
    
    /// <summary>
    /// Adds a child ui element.
    /// </summary>
    /// <param name="element">Element that will be added.</param>
    void AddChildren(IUiElement element);
    
    /// <summary>
    /// Removes a child element.
    /// </summary>
    /// <param name="element">Element to remove from control.</param>
    /// <returns>true on success, otherwise false.</returns>
    bool RemoveChildren(IUiElement element);
    
    /// <summary>
    /// Clears all child elements. 
    /// </summary>
    void ClearChildren();
}