using RouteFinder.Data.Models;

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


    public static TimeOnly GetTotalTime(this IEnumerable<RouteSegment> segments,TimeOnly currentTime)
    {
        var firstSegment = segments.First();
        var warpedTime = currentTime;
        warpedTime = firstSegment.Transport.WhenWillBeAt(firstSegment.From, warpedTime).Value;

        foreach (var item in segments.ShiftAndZipWithSelf())
        {
            warpedTime = warpedTime.Add(item.First.TravelTime.ToTimeSpan());

            if (item.First.Transport.Equals(item.Second.Transport)
                || item.Second.Equals(firstSegment)) continue;
            // if not same transport =>
            warpedTime = item.Second.Transport.WhenWillBeAt(item.Second.From, warpedTime).GetValueOrDefault();
        }

        return warpedTime.Add(-currentTime.ToTimeSpan());
    }

}
