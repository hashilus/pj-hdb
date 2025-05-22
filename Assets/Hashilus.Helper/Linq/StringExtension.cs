using System;
using System.Collections.Generic;
using System.Linq;

public static class StringExtension
{
    public static string JoinString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source.ToArray());
    }

    public static string JoinString<SourceT>(this IEnumerable<SourceT> source, string separator, Func<SourceT, string> pred)
    {
        return string.Join(separator, source.Select(pred).ToArray());
    }
}
