namespace BMS.Core.Extensions;
public static class EnumerableExtensions
{
    public static void Each<T>(this IEnumerable<T> instance, Action<T, int> action)
    {
        int index = 0;
        foreach (T item in instance)
            action(item, index++);
    }

    public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
    {
        if (instance != null)
        {
            foreach (T item in instance)
                action(item);
        }
    }
}

