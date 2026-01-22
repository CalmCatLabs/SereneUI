using System;
using System.IO;
using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Serene.Common.Extensions;
using SereneUI.Base;
using SereneUI.ContentControls;
using SereneUI.Extensions;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

/// <summary>
/// Creates a page. 
/// </summary>
/// <param name="services">IServiceProvider</param>
/// <param name="buildService">The builder service.</param>
[BuilderTargetType(typeof(Page))]
public class PageBuilder(IServiceProvider services, BuildService buildService) : IUiElementBuilder
{
    /// <inheritdoc />
    public object? CreateUiElement(Game game, ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var page = new Page
        {
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(page, node);

        if (node.Attributes.TryGetValue("Stylesheet", out string? stylesheetName) && !string.IsNullOrEmpty(stylesheetName))
        {
            var cssParser = new StylesheetParser(
                includeUnknownRules: true,
                includeUnknownDeclarations: true,
                tolerateInvalidSelectors: true,
                tolerateInvalidValues: true,
                tolerateInvalidConstraints: true);

            using var filestream = File.Open(stylesheetName, FileMode.Open);
            page.Stylesheet = cssParser.Parse(filestream);
            page.Stylesheet.CompileRules();
        }

        if (viewModel is null && node.Attributes.TryGetValue("DataContextType", out string? dataContextTypeName))
        {
            var dataContextType = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetType(dataContextTypeName) != null)?.GetType(dataContextTypeName);
            if (dataContextType != null) page.DataContext = services.GetService(dataContextType);
        }
        
        node.Children.ForEach(child =>
        {
            if (buildService.CreateUiElement(game, content, child, page.Stylesheet, page.DataContext) is IUiElement uiElement)
            {
                page.AddChildren(uiElement);
            }
        });
        
        // WireUp
        BindingEngine.WireUp(page);
        
        // WireUp Events
        WireUpFocusableElements(page, page);
        page.CompileObjectSelectors();
        NodeUtility.ForAllNodesRun(page, uiElement =>
        {
            uiElement.ApplyStyle();
            if (uiElement is UiElementBase { IsDraggable: true, IsEnabled: true } ueDrag)
            {
                ueDrag.Drag += page.OnDragHandler;
                ueDrag.DragEnter += page.OnDragEnterHandler;
                ueDrag.DragLeave += page.OnDragLeaveHandler;
            }
        });

        game.Window.TextInput += page.TextInputHandler;
        return page;
    }

    private void WireUpFocusableElements(Page page, IUiElement currentElement)
    {
        if (currentElement.IsFocusable)
        {
            page.AddFocusableElement(currentElement);
        }

        if (currentElement is ContentControlBase ccb && ccb.Content is not null)
        {
            WireUpFocusableElements(page, ccb.Content);
        }
        
        if (currentElement is ItemsControlBase icb)
        {
            icb.Children.ForEach(child =>
            {
                WireUpFocusableElements(page, child);
            });
        }
    }
}