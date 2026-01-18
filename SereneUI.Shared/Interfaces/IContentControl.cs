using SereneUI.Interfaces;

namespace SereneUI.Shared.Interfaces;

/// <summary>
/// Interface for a control with one child as content. 
/// </summary>
public interface IContentControl : IUiElement
{
    /// <summary>
    /// Content of the current control.
    /// </summary>
    IUiElement? Content { get; set; }
}