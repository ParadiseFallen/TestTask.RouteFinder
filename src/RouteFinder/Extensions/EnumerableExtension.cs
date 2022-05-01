namespace RouteFinder.Extensions;
public static class EnumerableExtension
{
    public static IEnumerable<(T First, T Second)> ShiftAndZipWithSelf<T>(
        this IEnumerable<T> enumerable, int shift = 1)
    {
        var toList = enumerable.Skip(shift).ToList();
        var endElements = enumerable.Take(shift).Reverse();
        toList.AddRange(endElements);
        return enumerable.Zip(toList);
    }
}
