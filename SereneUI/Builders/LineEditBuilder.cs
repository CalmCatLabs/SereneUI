using System.Collections.Generic;
using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework.Content;
using SereneUI.ContentControls;
using SereneUI.Shared.Attributes;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;
using HorizontalAlignment = SereneUI.Shared.Enums.HorizontalAlignment;
using VerticalAlignment = SereneUI.Shared.Enums.VerticalAlignment;

namespace SereneUI.Builders;

[BuilderTargetType(typeof(LineEdit))]
public class LineEditBuilder(BuildService buildService): IUiElementBuilder
{
    public object? CreateUiElement(ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var lineEdit = new LineEdit
        {
            Stylesheet = stylesheet,
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        BuilderCommon.SetCommonAttributes(lineEdit, node);
        node.Children.Add(new UiNode
        {
            TagName = "TextBlock",
            InnerText = "Hallo Welt",
            Attributes = new ()
            {
                {"Class", ".line-edit"}
            },
        });
        
        var textBlock = buildService.CreateUiElement(content, node.Children.First(), stylesheet, viewModel) as TextBlock;
        if (textBlock != null)
        {
            BuilderCommon.SetCommonAttributes(textBlock, node.Children.First());
            textBlock.Parent = lineEdit;
            lineEdit.Content = textBlock;
            textBlock.Stylesheet = stylesheet;
            textBlock.ApplyStyle();
        }

        lineEdit.ApplyStyle();
        
        return lineEdit;
    }
}