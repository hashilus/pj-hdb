using System;
using System.Collections.Generic;

public static class DictionaryExtension
{
    public static ValueT GetOrDefault<KeyT, ValueT>(this Dictionary<KeyT, ValueT> dict, KeyT key)
    {
        ValueT v;
        if (dict.TryGetValue(key, out v))
        {
            return v;
        }

        return default(ValueT);
    }

    public static ValueT GetOrDefault<KeyT, ValueT>(this Dictionary<KeyT, ValueT> dict, KeyT key, ValueT defaultValue)
    {
        ValueT v;
        if (dict.TryGetValue(key, out v))
        {
            return v;
        }

        return defaultValue;
    }

    public static ValueT GetOrCreate<KeyT, ValueT>(this Dictionary<KeyT, ValueT> dict, KeyT key, Func<ValueT> creator)
    {
        ValueT v;
        if (dict.TryGetValue(key, out v))
        {
            return v;
        }

        var newValue = creator();

        dict.Add(key, newValue);

        return newValue;
    }

    public static void ForEachPair<KeyT, ValueT>(this Dictionary<KeyT, ValueT> dict, Action<KeyT, ValueT> predicate)
    {
        foreach (var pair in dict)
        {
            predicate(pair.Key, pair.Value);
        }
    }
}
