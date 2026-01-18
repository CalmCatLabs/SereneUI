using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Serene.Common.Abstracts;

public abstract class SceneBase : IDisposable
{

    private bool _isLoaded = false;
    private bool _isDisposed = false;
    protected GraphicsDevice GraphicsDevice { get; private set; } = null!;
    protected IServiceProvider Services { get; private set; } = null!;

    protected Game Game { get; set; } = null!;

    public void EnsureLoaded(GraphicsDevice graphicsDevice, IServiceProvider services)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_isLoaded) return;

        GraphicsDevice = graphicsDevice;
        Services = services;
        Game = services.GetRequiredService<Game>();

        LoadContent();
        _isLoaded = true;
    }

    public abstract void LoadContent();
    public abstract void UnloadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                UnloadContent();
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}