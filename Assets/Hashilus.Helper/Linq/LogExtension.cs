using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class LogExtension
{
    public static void Log<T>(this IEnumerable<T> source, Func<T, object> predicate, string firstLine = null)
    {
        var sb = new StringBuilder();
        if (firstLine != null) sb.AppendLine(firstLine);

        int i = 0;
        foreach (var element in source)
        {
            try
            {
                sb.AppendLine("[" + i + "] " + predicate(element).ToString());
            }
            catch (Exception e)
            {
                sb.AppendLine("[" + i + "] " + e);
            }
            i++;
        }

        Debug.Log(sb.ToString());
    }

    public static void LogEach<T>(this IEnumerable<T> source, Func<T, object> predicate, string firstLine = null)
    {
        if (firstLine != null) Debug.Log(firstLine);

        int i = 0;
        foreach (var element in source)
        {
            try
            {
                Debug.Log("[" + i + "] " + predicate(element).ToString());
            }
            catch (Exception e)
            {
                Debug.LogError("[" + i + "] " + e);
            }
            i++;
        }
    }

    public static IEnumerable<T> LogThen<T>(this IEnumerable<T> source, Func<T, object> predicate, string firstLine = null)
    {
        var sb = new StringBuilder();
        if (firstLine != null) sb.AppendLine(firstLine);

        int i = 0;
        foreach (var element in source)
        {
            try
            {
                sb.AppendLine("[" + i + "] " + predicate(element).ToString());
            }
            catch (Exception e)
            {
                sb.AppendLine("[" + i + "] " + e);
            }
            yield return element;
            i++;
        }

        Debug.Log(sb.ToString());
    }

    public static IEnumerable<T> LogEachThen<T>(this IEnumerable<T> source, Func<T, object> predicate, string firstLine = null)
    {
        if (firstLine != null) Debug.Log(firstLine);

        int i = 0;
        foreach (var element in source)
        {
            try
            {
                Debug.Log("[" + i + "] " + predicate(element).ToString());
            }
            catch (Exception e)
            {
                Debug.LogError("[" + i + "] " + e);
            }
            yield return element;
            i++;
        }
    }
}
