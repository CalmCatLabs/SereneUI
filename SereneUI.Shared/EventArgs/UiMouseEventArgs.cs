using System;
using Microsoft.Xna.Framework;
using SereneUI.Shared.DataStructures;

namespace SereneUI.Shared.EventArgs;

/// <summary>
/// EventArgs object for mouse events, position and states.
/// </summary>
/// <param name="position">Current mouse position.</param>
/// <param name="inputData">InputStates.</param>
public class UiMouseEventArgs(Point position, UiInputData inputData) : System.EventArgs
{
    public Point Position { get; } = position;
    public UiInputData InputData { get; } = inputData;
}