
using System;
using System.Collections.Generic;

public static class LinqExtend
{
    public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach(var x in items)
            source.Add(x);
        return source;
    } 
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source, 
        Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}