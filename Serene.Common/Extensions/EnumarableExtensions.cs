using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Serene.Common.Extensions;

public static class EnumarableExtensions
{
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        foreach (var item in enumerable)
        {
            try
            {
                action(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}