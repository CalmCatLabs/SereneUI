using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Serene.Common.Extensions;
using SereneUI.Base;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Utilities;

public static class BindingEngine
{
    public static void WireUp(UiElementBase root)
    {
        Traverse(root, element =>
        {
            foreach (var (targetPropertyName, expr) in element.MarkupExpressions)
            {
                if (TryParseBinding(expr, out var path))
                    Bind(element, targetPropertyName, path);

                if (TryParseCommand(expr, out var commandName))
                    BindCommand(element, targetPropertyName, commandName);
            }
        });
    }

    private static void Bind(IUiElement element, string targetProperty, string path)
    {
        if (element.DataContext is null) return;

        var dc = element.DataContext;
        var dcType = dc.GetType();

        var targetProp = element.GetType().GetProperty(targetProperty, BindingFlags.Instance | BindingFlags.Public);
        var sourceProp = dcType.GetProperty(path, BindingFlags.Instance | BindingFlags.Public);

        if (targetProp is null || sourceProp is null) return;

        void Update()
        {
            var value = sourceProp.GetValue(dc);
            targetProp.SetValue(element, value);
            element.InvalidateVisual();
            element.InvalidateMeasure();
            element.InvalidateArrange();
        }

        Update();

        if (dc is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == path || string.IsNullOrEmpty(e.PropertyName))
                    Update();
            };
        }
    }

    private static void BindCommand(IUiElement element, string targetProperty, string commandName)
    {
        if (element.DataContext is null) return;

        var dc = element.DataContext;
        var dcType = dc.GetType();

        // Ziel: Property auf dem UI-Element (z.B. OnMouseEnter)
        var targetProp = element.GetType().GetProperty(targetProperty, BindingFlags.Instance | BindingFlags.Public);
        if (targetProp is null) return;

        // 1) Property im VM suchen (Command)
        var vmProp = dcType.GetProperty(commandName, BindingFlags.Instance | BindingFlags.Public);
        var cmdObj = vmProp?.GetValue(dc);

        // 2) Fallback: Methode im VM
        var method = dcType.GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Falls direkt ein ICommand vorhanden ist -> setzen
        if (cmdObj is ICommand cmd)
        {
            targetProp.SetValue(element, cmd);
            return;
        }

        // Falls Methode vorhanden -> Wrapper-RelayCommand bauen
        if (method is not null && targetProp.PropertyType.IsAssignableFrom(typeof(ICommand)))
        {
            var wrapper = new RelayCommand(
                execute: (sender, args) =>
                {
                    // Unterstütze Signaturen: (), (object? sender), (object? sender, object? args)
                    var ps = method.GetParameters();
                    if (ps.Length == 0) method.Invoke(dc, null);
                    else if (ps.Length == 1) method.Invoke(dc, new[] { sender });
                    else method.Invoke(dc, new[] { sender, args });
                },
                canExecute: (_, __) => true
            );

            targetProp.SetValue(element, wrapper);
        }
    }

    private static bool TryParseBinding(string expr, out string path)
    {
        path = "";
        // akzeptiere "{Binding Playername}" oder "{Binding Path=Playername}"
        expr = expr.Trim();
        if (!expr.StartsWith("{Binding", StringComparison.OrdinalIgnoreCase)) return false;

        expr = expr.Trim('{', '}').Trim();
        var parts = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // simpel: "{Binding Playername}"
        if (parts.Length >= 2) { path = parts[1].Trim(); return true; }

        // etwas weniger simpel: "Path=Playername"
        var pathPart = parts.FirstOrDefault(p => p.StartsWith("Path=", StringComparison.OrdinalIgnoreCase));
        if (pathPart is not null) { path = pathPart.Split('=')[1].Trim(); return true; }

        return false;
    }

    private static bool TryParseCommand(string expr, out string name)
    {
        name = "";
        expr = expr.Trim();
        if (!expr.StartsWith("{Command", StringComparison.OrdinalIgnoreCase)) return false;

        expr = expr.Trim('{', '}').Trim();
        var parts = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2) { name = parts[1].Trim(); return true; }

        return false;
    }

    private static void Traverse(IUiElement node, Action<IUiElement> visit)
    {
        visit(node);
        if (node is ContentControlBase contentElement && contentElement.Content is not null)
        {
            Traverse(contentElement.Content, visit);
        }

        if (node is ItemsControlBase itemsControl)
        {
            itemsControl.Children.ForEach(child =>
            {
                Traverse(child, visit);
            });
        }
        
        // foreach (var c in node.Children) Traverse(c, visit);
    }
}