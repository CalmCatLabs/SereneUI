
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

public abstract class ContentControlBase : UiElementBase, IContentControl
{
    public IUiElement? Content { get; set; }
}