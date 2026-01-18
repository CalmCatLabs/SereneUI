
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

/// <summary>
/// Base for a control that can have on single child as Content.
/// </summary>
public abstract class ContentControlBase : UiElementBase, IContentControl
{
    /// <summary>
    /// Child element.
    /// </summary>
    public IUiElement? Content { get; set; }
}