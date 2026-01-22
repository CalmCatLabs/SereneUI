using System.Collections.Generic;
using System.Linq;
using ExCSS;
using Microsoft.Xna.Framework;
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
    public object? CreateUiElement(Game game, ContentManager content, UiNode node, Stylesheet? stylesheet, object? viewModel)
    {
        var lineEdit = new LineEdit
        {
            Stylesheet = stylesheet,
            DataContext = viewModel,
            MarkupExpressions = node.MarkupExpressions
        };
        node.Children.Add(new UiNode
        {
            TagName = "TextBlock",
            Attributes = new ()
            {
                {"Class", "line_edit"}
            },
        });
        BuilderCommon.SetCommonAttributes(lineEdit, node);
        
        var textBlock = buildService.CreateUiElement(game, content, node.Children.First(), stylesheet, viewModel) as TextBlock;
        if (textBlock != null)
        {
            BuilderCommon.SetCommonAttributes(textBlock, node.Children.First());
            textBlock.Parent = lineEdit;
            lineEdit.Content = textBlock;
            textBlock.Stylesheet = stylesheet;
            textBlock.Text = lineEdit.Placeholder;
            textBlock.ApplyStyle();
        }

        lineEdit.ApplyStyle();
        return lineEdit;
    }
}