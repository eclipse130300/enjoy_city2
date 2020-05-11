using System.Collections;

public static class CollectionUtil
{
    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }
}