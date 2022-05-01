using RouteFinder.Abstractions.Services.Routing;
using RouteFinder.Data.Models;
using RouteFinder.Extensions;

namespace RouteFinder.Services.Routing;
public class TimeRespectWeighter : IRouteSegmentWeigher
{
    private TimeOnly CurrentTime { get; init; }
    public TimeRespectWeighter(TimeOnly currentTime) =>
        CurrentTime = currentTime;

    public ulong MeasureCost(RouteSegment currentSegment, RouteSegment? previousSegment = null, ulong currentCost = 0)
    {
        TimeOnly waitTime = new(0, 0);

        if (!previousSegment?.Transport.Equals(currentSegment.Transport) ?? true)
        {
            var whenWillBeAtThisStop = previousSegment?.Transport.WhenWillBeAt(currentSegment.From, CurrentTime) ?? CurrentTime;
            var nextTransportTime = currentSegment.Transport.WhenWillBeAt(currentSegment.From, whenWillBeAtThisStop);
            waitTime = nextTransportTime!.Value.Add(-whenWillBeAtThisStop.ToTimeSpan());
        }

        return currentCost
            + (ulong)(waitTime.Minute)
            + (ulong)(currentSegment.TravelTime.Minute)
            + 1;
    }
}
