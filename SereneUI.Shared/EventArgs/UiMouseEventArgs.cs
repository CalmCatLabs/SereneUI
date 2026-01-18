using System;
using Microsoft.Xna.Framework;
using SereneUI.Shared.DataStructures;

namespace SereneUI.Shared.EventArgs;

public class UiMouseEventArgs(Point position, UiInputData inputData) : System.EventArgs
{
    public Point Position { get; } = position;
    public UiInputData InputData { get; } = inputData;
}