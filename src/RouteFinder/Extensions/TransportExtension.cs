using RouteFinder.Data.Models;

namespace RouteFinder.Extensions;
public static class TransportExtension
{

    public static TimeOnly? WhenWillBeAt(
        this Transport transport,
        RouteStop routeStop,
        TimeOnly currentTime)
    {
        if (!transport
            .RouteSegments
            .Select(x => x.To)
            .Contains(routeStop))
            return null;

        var transportLoopTime = transport.LoopTime;

        var segmentsUntilCurrentStop = transport
            .RouteSegments
            .TakeWhile(x => !x.From.Equals(routeStop));

        var travelTimeFromStartToTargetStop = 
            segmentsUntilCurrentStop.Any()
            ? segmentsUntilCurrentStop.Select(x => x.TravelTime)
                .Aggregate((prev,current)=> 
                    prev.Add(current.ToTimeSpan()))
            : default;

        var warpedTime = travelTimeFromStartToTargetStop
            .Add(transport.StartTime.ToTimeSpan());

        if(currentTime.IsBetween(transport.StartTime,transport.EndTime))
        {
            while (warpedTime < currentTime)
                warpedTime = warpedTime.Add(transportLoopTime.ToTimeSpan());

            return warpedTime;
        }
        return transport.StartTime.Add(travelTimeFromStartToTargetStop.ToTimeSpan());
    }
}
