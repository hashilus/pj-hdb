using System;
using System.Collections.Generic;

public static class DefaultLinqExtension
{
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
        foreach (var element in source)
        {
            return element;
        }

        return defaultValue;
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue, Func<TSource, Boolean> predicate)
    {
        foreach (var element in source)
        {
            if (predicate(element)) return element;
        }

        return defaultValue;
    }
}
