using Microsoft.Xna.Framework;

namespace SereneUI.Shared.DataStructures;

public readonly record struct  UiInputData(
    Point MousePosition,
    bool LeftMouseDown,
    bool LeftMousePressed,
    bool LeftMouseReleased,
    bool RightMouseDown,
    bool RightMousePressed,
    bool RightMouseReleased,
    int ScrollDelta
    );