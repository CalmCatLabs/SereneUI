using System.Collections.Generic;
using System.Linq;
using ExCSS;
using Serene.Common.Extensions;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Interfaces;
using SereneUI.Utilities;

namespace SereneUI.Extensions;

public static class StylesheetExtension
{
    private static Dictionary<Stylesheet, List<CompiledRule>> _compiledRulesPerStylesheet = [];
    private static Dictionary<IUiElement, SimpleCssSelector> _uiElementSelectors = [];

    public static List<IStyleRule?> MatchingRules(this IUiElement uiElement, Stylesheet stylesheet)
    {
        var rules = new List<IStyleRule?>();

        if (!_compiledRulesPerStylesheet.TryGetValue(stylesheet, out var stylesheetRules) || stylesheetRules.Count == 0)
        {
            return rules;
        }

        if (!_uiElementSelectors.TryGetValue(uiElement, out var uiElementSelector))
        {
            return rules;
        }
        
        var compiledRules = stylesheetRules
            .Where(sr => sr.Selector is not null && uiElementSelector.Implies(sr.Selector) && sr.Rule is not null)
            .ToList();
        compiledRules.Sort(CompareSpecificity);
        rules = compiledRules.Select(sr => sr.Rule).ToList();
        
        return rules;
    }

    private static int CompareSpecificity(CompiledRule left, CompiledRule right)
    {
        if (left.Specificity is null || right.Specificity is null) return 0;
        
        int compareResult = 0;
        if (left.Specificity.IdCount !=  right.Specificity.IdCount) compareResult += (left.Specificity.IdCount - right.Specificity.IdCount) * 1000;
        if (left.Specificity.ClassesCount != right.Specificity.ClassesCount) compareResult += (left.Specificity.ClassesCount - right.Specificity.ClassesCount) * 100;
        if (left.Specificity.PseudoClassesCount != right.Specificity.PseudoClassesCount) compareResult += (left.Specificity.PseudoClassesCount - right.Specificity.PseudoClassesCount) * 10;
        if (left.Specificity.TagsCount != right.Specificity.TagsCount) compareResult += left.Specificity.TagsCount - right.Specificity.TagsCount;
        return compareResult;
    }

    public static void CompileObjectSelectors(this IUiElement current)
    {
        NodeUtility.ForAllNodesRun(current, uiElement =>
        {

            var classes = uiElement.Class?.Split(' ')
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c =>
                {
                    return c.TrimStart('.');
                });
            var pseudos = uiElement.PseudoClass?.Split(' ')
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c =>
                {
                    return c.TrimStart(':');
                });
            
            var selector = new SimpleCssSelector
            {
                TagName = uiElement.GetType().Name,
                Id = uiElement.Id?.TrimStart('#'),
                Classes = [..classes ?? []],
                Pseudos = [..pseudos ?? []]
            };
            _uiElementSelectors.Remove(uiElement);
            _uiElementSelectors.Add(uiElement, selector);
        });
    }
    
    public static void CompileRules(this Stylesheet stylesheet)
    {
        int count = 0;
        if (!_compiledRulesPerStylesheet.TryGetValue(stylesheet, out var compiledRules))
        {
            compiledRules = new List<CompiledRule>();
            _compiledRulesPerStylesheet.Add(stylesheet, compiledRules);
        }
        stylesheet.StyleRules.ForEach(styleRule =>
        {
            var selector = new SimpleCssSelector(styleRule.SelectorText);
            var specificity = new Specificity();
            if (selector.Classes != null) specificity.ClassesCount += selector.Classes.Count;
            if (selector.Pseudos != null) specificity.PseudoClassesCount += selector.Pseudos.Count;

            if (!string.IsNullOrWhiteSpace(selector.Id)) specificity.IdCount = 1;
            if (!string.IsNullOrWhiteSpace(selector.TagName)) specificity.TagsCount = 1;

            var compiledRule = new CompiledRule
            {
                Order = count++,
                Rule = styleRule,
                Selector = selector,
                Specificity = specificity,
                SelectorText = styleRule.SelectorText,
            };
            compiledRules.Add(compiledRule);
        });
    }
}