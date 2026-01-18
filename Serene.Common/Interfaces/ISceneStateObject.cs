using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serene.Common.Interfaces;

public interface ISceneStateObject
{
    public void Initialize();
    public void Draw(GraphicsDevice graphicsDevice, GameTime gameTime, SpriteBatch spriteBatch, ICameraService camera);
    public void Update(GraphicsDevice graphicsDevice, GameTime gameTime, ICameraService camera);
}