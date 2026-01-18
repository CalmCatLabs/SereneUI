using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Serene.Common.Helpers;

public class RandomIdGenerator
{
    private static List<string> _elements = [];
    public static string Get(int length=8, string? prefix = null)
    {
        var chars = string.Empty;
        do
        {
            chars = Enumerable.Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(length)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c), sb => sb.ToString());
        } while (_elements.Contains(prefix + chars));
        _elements.Add(prefix + chars);
        return prefix + chars;
    }
}