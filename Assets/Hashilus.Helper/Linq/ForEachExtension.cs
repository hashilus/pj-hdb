using System;
using System.Collections.Generic;

public static class ForEachExtension
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> predicate)
    {
        foreach (var element in source)
        {
            predicate(element);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> predicate)
    {
        var i = 0;
        foreach (var element in source)
        {
            predicate(element, i);
            i++;
        }
    }
}
