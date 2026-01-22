using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SereneUI.Shared.DataStructures;

/// <summary>
/// Helper struct for input data. 
/// </summary>
/// <param name="MousePosition"></param>
/// <param name="LeftMouseDown"></param>
/// <param name="LeftMousePressed"></param>
/// <param name="LeftMouseReleased"></param>
/// <param name="RightMouseDown"></param>
/// <param name="RightMousePressed"></param>
/// <param name="RightMouseReleased"></param>
/// <param name="ScrollDelta"></param>
public readonly record struct  UiInputData(
    Point MousePosition,
    bool LeftMouseDown,
    bool LeftMousePressed,
    bool LeftMouseReleased,
    bool RightMouseDown,
    bool RightMousePressed,
    bool RightMouseReleased,
    int ScrollDelta,
    KeyboardStateUtility? KeyboardState = null
    );