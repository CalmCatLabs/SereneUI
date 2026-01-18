using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.Shared.DataStructures;

namespace SereneUI.Shared.Interfaces;

public interface IUiElementBuilder
{
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel);
}