using System.Collections.Generic;
using SereneUI.Shared.Enums;

namespace SereneUI.Shared.DataStructures;

/// <summary>
/// Helper for reading the xml in a structured way. 
/// </summary>
public class UiNode
{
    /// <summary>
    /// TagName for the ui control. (it's the class name on c# site.)
    /// </summary>
    public string TagName { get; set; } = null!;
    
    /// <summary>
    /// Attributes of the element.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = [];
    
    /// <summary>
    /// List of child nodes. 
    /// </summary>
    public List<UiNode> Children { get; set; } = [];
    
    /// <summary>
    /// InnerText if any, and no children.
    /// </summary>
    public string? InnerText { get; set; }
    
    /// <summary>
    /// Shorthand to the same attribute. 
    /// </summary>
    public string? FontReference { get; set; }
    
    /// <summary>
    /// Dictionary of recognized MarkupExpressions like {Binding ...} oder {Command ...}
    /// </summary>
    public Dictionary<string, string> MarkupExpressions { get; } = [];
}