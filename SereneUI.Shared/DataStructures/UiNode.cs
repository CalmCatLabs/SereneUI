using System.Collections.Generic;
using SereneUI.Shared.Enums;

namespace SereneUI.Shared.DataStructures;

public class UiNode
{
    public string Type { get; set; } = null!;
    public Dictionary<string, string> Attributes { get; set; } = [];
    public List<UiNode> Children { get; set; } = [];
    public string? InnerText { get; set; }
    public List<UiNode>? InnerChildren { get; set; }
    public string? FontReference { get; set; }
    
    // Target / Expression?
    public Dictionary<string, string> MarkupExpressions { get; } = [];
}