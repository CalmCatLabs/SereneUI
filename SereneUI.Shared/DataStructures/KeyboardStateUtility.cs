using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace SereneUI.Shared.DataStructures;

public class KeyboardStateUtility(KeyboardState keyboardState) : ObservableObject
{
    private Keys[] _pressedKeys = [];
    private Keys[] _lastPressedKeys = [];
    public KeyboardState KeyboardState { get; set => SetProperty(ref field, value, UpdateKeyStates); } = keyboardState;

    public void UpdateKeyStates()
    {
        _lastPressedKeys = _pressedKeys;
        _pressedKeys = KeyboardState.GetPressedKeys();
    }

    public IEnumerable<Keys> GetReleasedKeys()
    {
        var result = _lastPressedKeys.Except(_pressedKeys);
        // _lastPressedKeys = _pressedKeys;
        // _pressedKeys = [];
        return result;
    }

    public bool IsShiftPressed()
    {
        return  KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
    }

    public bool IsCtrlPressed()
    {
        return KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
    }
}