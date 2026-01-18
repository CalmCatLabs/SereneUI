using SereneUI.Interfaces;

namespace SereneUI.Shared.Interfaces;

public interface IContentControl : IUiElement
{
    IUiElement? Content { get; set; }
}