using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serene.Common.Interfaces;

namespace Serene.Common.Events;

public class ClickHandlerEventArgs(
    GraphicsDevice graphicsDevice,
    GameTime gameTime,
    ICameraService camera)
    : EventArgs
{
    public GraphicsDevice GraphicsDevice { get; init; } = graphicsDevice;
    public GameTime GameTime { get; init; } = gameTime;
    public ICameraService Camera { get; init; } = camera;
}
