using System.Collections;
using System.Collections.Generic;

public static class EnumeratorExtensions
{
    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }
    public static IEnumerable<object> ToEnumerable(this IEnumerator enumerator)
    {
        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }
}