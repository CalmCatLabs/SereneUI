using System;
using System.IO;
using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Builders;

[BuilderTargetType(typeof(Page))]
public class PageBuilder(IServiceProvider services, BuildService buildService) : IUiElementBuilder
{
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var page = new Page
        {
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(page, node);
        page.ApplyStyle();

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
            page.ApplyStyle();
        }

        if (viewModel is null && node.Attributes.TryGetValue("DataContextType", out string? dataContextTypeName))
        {
            var dataContextType = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetType(dataContextTypeName) != null)?.GetType(dataContextTypeName);
            if (dataContextType != null) page.DataContext = services.GetService(dataContextType);
        }
        
        node.Children.ForEach(child =>
        {
            if (buildService.CreateUiElement(content, child, page.Stylesheet, page.DataContext) is IUiElement uiElement)
            {
                page.AddItem(uiElement);
            }
        });
        
        // WireUp
        BindingEngine.WireUp(page);
        
        return page;
    }
}