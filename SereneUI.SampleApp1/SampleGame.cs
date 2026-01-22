using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SereneUI.Extensions;
using SereneUI.Shared.Enums;
using SereneUI.SampleApp1.ViewModels;

namespace SereneUI.SampleApp1;

public class SampleGame : Game
{
    private SpriteBatch _spriteBatch;
    private SereneUiSystem _sereneUi;
    private readonly ServiceProvider _serviceProvider;

    public SampleGame()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Game, SampleGame>(_ => this);
        services.AddSingleton<SampleGame>(_ => this);
        
        services.AddSingleton<MainViewModel>();
        services.AddSereneUi();

        _serviceProvider = services.BuildServiceProvider();
        
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 1024;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _sereneUi = _serviceProvider.GetRequiredService<SereneUiSystem>();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _sereneUi.Initialize(_spriteBatch);
        _sereneUi.Load("./Assets/Views/main.ui.xml");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape) || _serviceProvider.GetRequiredService<MainViewModel>().RequestExit)
            Exit();
        
        _sereneUi.Update(gameTime, _spriteBatch);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_serviceProvider.GetRequiredService<MainViewModel>().ClearColor);

        _spriteBatch.Begin();
        _sereneUi.Draw(gameTime, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}