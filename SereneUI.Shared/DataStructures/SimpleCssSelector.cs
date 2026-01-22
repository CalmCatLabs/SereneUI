using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExCSS;
using Serene.Common.Extensions;

namespace SereneUI.Shared.DataStructures;


public class CompiledRule
{
    public string? SelectorText { get; set; }
    public SimpleCssSelector? Selector { get; set; }
    public Specificity? Specificity { get; set; }
    public int Order { get; set; }
    public IStyleRule? Rule { get; set; }
}

public class SimpleCssSelector
{
    public string? TagName { get; set; }
    public string? Id { get; set; }
    public HashSet<string>? Classes { get; set; }
    public HashSet<string>? Pseudos { get; set; }
    
    public SimpleCssSelector()
    {
    }
    public SimpleCssSelector(string selectorString)
    {
        ParseSelectorString(selectorString);
    }
    
    
    public bool Implies(SimpleCssSelector? other)
    {
        if (other == null) return false;

        if (!string.IsNullOrWhiteSpace(other.TagName))
        {
            if (!string.Equals(TagName, other.TagName, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(other.Id))
        {
            if (!string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (other.Classes is { Count: > 0 })
        {
            if (Classes is null) return false;
            if (!Classes.IsSupersetOf(other.Classes)) return false;
        }

        if (other.Pseudos is { Count: > 0 })
        {
            if (Pseudos is null) return false;
            if (!Pseudos.IsSupersetOf(other.Pseudos)) return false;
        }
        
        return true;
    }
    
    private void ParseSelectorString(string selectorString)
    {
        TagName = GetTagName(selectorString);
        Id = GetId(selectorString);
        Classes = GetClassNames(selectorString);
        Pseudos = GetPseudoClasses(selectorString);
    }

    private string? GetTagName(string selectorString)
    {
        var regEx = Regex.Match(selectorString, "(?<![#.:[])\\b([a-zA-Z][a-zA-Z0-9_-]*)\\b");
        if (regEx.Success)
        {
            return regEx.Value.Trim();
        }

        return string.Empty;
    }

    private HashSet<string>? GetPseudoClasses(string selectorString)
    {
        var classMatches = Regex.Matches(selectorString, "\\:\\w+");
        if (classMatches.Count == 0) return null;
        var classNames = new HashSet<string>();
        classMatches.ForEach(match =>
        {
            classNames.Add(match.Value.Trim().TrimStart(':'));
        });
        return classNames;
    }

    private HashSet<string>? GetClassNames(string selectorString)
    {
        var classMatches = Regex.Matches(selectorString, "\\.\\w+");
        if (classMatches.Count == 0) return null;
        var classNames = new HashSet<string>();
        classMatches.ForEach(match =>
        {
            classNames.Add(match.Value.Trim().TrimStart('.'));
        });
        return classNames;
    }

    private string? GetId(string selectorString)
    {
        var regEx = Regex.Match(selectorString, "#\\w+");
        return regEx.Success ? regEx.Value.TrimStart('#') : null;
    }
}

public class Specificity : IComparable<Specificity>
{
    public int IdCount { get; set; } = 0;
    public int ClassesCount { get; set; } = 0;
    public int PseudoClassesCount { get; set; } = 0;
    public int TagsCount { get; set; } = 0;
    
    public int ValuesCount => ParseToInt();

    public int CompareTo(Specificity? other)
    {
        var thisValue = this.ParseToInt();
        var otherValue = other?.ParseToInt() ?? 0;
        return thisValue < otherValue ? -1 : thisValue > otherValue ? 1 : 0;
    }

    private int ParseToInt()
    {
        var toParse = $"{IdCount}{ClassesCount}{TagsCount}{PseudoClassesCount}";
        if (int.TryParse(toParse, out var result))
        {
            return result;
        }
        throw new ParseException($"Could not parse '{toParse}' to int.");
    }
}