using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.Shared.DataStructures;

namespace SereneUI.Shared.Interfaces;

/// <summary>
/// Interface for a IUiElementBuilder 
/// </summary>
public interface IUiElementBuilder
{
    /// <summary>
    /// Creates a ui element, depending on the current implementation.
    /// </summary>
    /// <param name="content">Monogames content manager</param>
    /// <param name="node">Current ui node (that xml to dotnet converted)</param>
    /// <param name="stylesheet">ExCSS Stylesheet object.</param>
    /// <param name="viewModel">An optional view model/controler.</param>
    /// <returns></returns>
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel);
}