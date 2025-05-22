using System;
using System.Collections.Generic;
using System.Linq;

public static class SampleExtension
{
    static Random random = new Random();

    public static SourceT Sample<SourceT>(this IEnumerable<SourceT> source)
    {
        var count = source.Count();
        return source.ElementAt(random.Next(count));
    }
}
