using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serene.Common.Interfaces;

public interface IGameObject
{
    void Initialize();
    void Update(GameTime gameTime);
    public void Draw(GraphicsDevice graphicsDevice, GameTime gameTime, ICameraService camera);
}