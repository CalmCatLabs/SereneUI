using System;
using Serene.Common.Extensions;
using SereneUI.Base;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Utilities;

public static class NodeUtility
{
    public static void ForAllNodesRun(IUiElement currentElement, Action<IUiElement> action)
    {
        action(currentElement);
        
        if (currentElement is ContentControlBase ccb && ccb.Content != null)
            ForAllNodesRun(ccb.Content, action);
        
        if (currentElement is ItemsControlBase icb && icb.Children.Count > 0)
            icb.Children.ForEach(child => ForAllNodesRun(child, action));
    }
}