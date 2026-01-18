using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serene.Common.Extensions;
using SereneUI.Builders;
using SereneUI.ContentControls;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Converters;

namespace SereneUI;

public class SereneUiSystem(Game game, BuildService buildService)
{
    private SpriteBatch? _spriteBatch = null;
    private RenderTarget2D? _uiScreenTarget = null;
    private Page? _currentPage = null;
    private bool _wasRightMousePressed = false;
    private bool _wasLeftMousePressed = false;

    public static Game Game { get; internal set; } 
    
    public void Initialize(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        ConverterService.Initialize();
        buildService.Initialize();
        
        CreateOrResizeUiRenderTarget();
        Game = game;
    }

    public void Update(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_uiScreenTarget is not null
            && (_uiScreenTarget.Width != game.GraphicsDevice.PresentationParameters.BackBufferWidth
                || _uiScreenTarget.Height != game.GraphicsDevice.PresentationParameters.BackBufferHeight))
        {
            CreateOrResizeUiRenderTarget();
        }

        if (_uiScreenTarget != null)
        {
            _currentPage?.Measure(new Point(_uiScreenTarget.Width, _uiScreenTarget.Height));
            _currentPage?.Arrange(new Rectangle(0, 0, 
                _uiScreenTarget.Width, 
                _uiScreenTarget.Height)
            );

            var mouseState = Mouse.GetState();
            var isLeftButtonDown = mouseState.LeftButton == ButtonState.Pressed && !_wasLeftMousePressed;
            var isLeftButtonPressed = mouseState.LeftButton == ButtonState.Pressed && _wasLeftMousePressed;
            var isLeftButtonUp = mouseState.LeftButton == ButtonState.Released && _wasLeftMousePressed;
            
            
            var isRightButtonDown = mouseState.RightButton == ButtonState.Pressed && !_wasRightMousePressed;
            var isRightButtonPressed = mouseState.RightButton == ButtonState.Pressed && _wasRightMousePressed;
            var isRightButtonUp = mouseState.RightButton == ButtonState.Released && _wasRightMousePressed;
            
            if (isLeftButtonDown) _wasLeftMousePressed = true;
            if (isLeftButtonUp) _wasLeftMousePressed = false;
            
            if (isLeftButtonDown) _wasRightMousePressed = true;
            if (isRightButtonUp) _wasRightMousePressed = false;
            
            var input = new UiInputData(
                mouseState.Position,
                isLeftButtonDown,
                isLeftButtonPressed,
                isLeftButtonUp,
                isRightButtonDown,
                isRightButtonPressed,
                isRightButtonUp,
                mouseState.ScrollWheelValue
                );
            _currentPage?.Update(gameTime, input);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_uiScreenTarget is null) return;
        _currentPage?.Draw(spriteBatch);
    }

    public void Load(string viewFileName, object? viewModel = null)
    {
        var viewDoc = XDocument.Load(viewFileName);
        if (viewDoc is null) throw new FileNotFoundException(viewFileName);
        if (viewDoc.Root is null) throw new FileLoadException(viewFileName);
        
        var nodes = MakeNodes(viewDoc.Root);
        var page = buildService.CreateUiElement(game.Content, nodes, null, viewModel) as Page;
        _currentPage = page ?? throw new FileLoadException(viewFileName);
    }

    private UiNode MakeNodes(XElement node)
    {
        var uiNode = new UiNode
        {
            TagName = node.Name.LocalName ?? throw new NullReferenceException($"{nameof(node.Name.LocalName)} was null.")
        };
        
        node.Attributes().ForEach(attr =>
        {
            if (!string.IsNullOrWhiteSpace(attr.Value) 
                && Regex.IsMatch(attr.Value, "\\{Binding( [a-zA-Z0-9.]*)*\\}", RegexOptions.IgnoreCase))
            {
                uiNode.MarkupExpressions.Add(attr.Name.LocalName, attr.Value);
            }
            if (!string.IsNullOrWhiteSpace(attr.Value) 
                && Regex.IsMatch(attr.Value, "\\{Command( [a-zA-Z0-9.]*)*\\}", RegexOptions.IgnoreCase))
            {
                uiNode.MarkupExpressions.Add(attr.Name.LocalName, attr.Value);
            }
            
            uiNode.Attributes.Add(attr.Name.LocalName, attr.Value);
            if (attr.Name.LocalName.Equals("Font", StringComparison.InvariantCultureIgnoreCase))
            {
                uiNode.FontReference = attr.Value;
            }
        });

        if (!node.HasElements && !string.IsNullOrEmpty(node.Value))
        {
            if (!string.IsNullOrWhiteSpace(node.Value) 
                && Regex.IsMatch(node.Value, "\\{Binding( [a-zA-Z0-9.]*)*\\}", RegexOptions.IgnoreCase))
            {
                uiNode.MarkupExpressions.Add("Text", node.Value);
            }
            if (!string.IsNullOrWhiteSpace(node.Value) 
                && Regex.IsMatch(node.Value, "\\{Command( [a-zA-Z0-9.]*)*\\}", RegexOptions.IgnoreCase))
            {
                uiNode.MarkupExpressions.Add(node.Name.LocalName, node.Value);
            }
            uiNode.InnerText = node.Value;
        }
        
        
        foreach (var element in node.Elements())
        {
            uiNode.Children.Add(MakeNodes(element));
        }
        
        return uiNode;
    }
    
    private void CreateOrResizeUiRenderTarget()
    {
        _uiScreenTarget?.Dispose();

        _uiScreenTarget = new RenderTarget2D(
            game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight,
            mipMap: false,
            preferredFormat: SurfaceFormat.Color,
            preferredDepthFormat: DepthFormat.None);
    }
}