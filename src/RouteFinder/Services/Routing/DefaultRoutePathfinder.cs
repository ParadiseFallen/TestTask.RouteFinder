using RouteFinder.Abstractions.Services.Routing;
using RouteFinder.Data.Models;

namespace RouteFinder.Services.Routing;
public class DefaultRoutePathfinder : IRouteFinder
{
    private Func<TimeOnly, IRouteSegmentWeigher> RouteSegmentWeigherFactory { get; init; }

    public DefaultRoutePathfinder(Func<TimeOnly, IRouteSegmentWeigher> routeSegmentWeigherFactory)
    {
        RouteSegmentWeigherFactory = routeSegmentWeigherFactory;
    }

    public Task<IEnumerable<RouteSegment>> FindRoute(
        RouteStop from,
        RouteStop to,
        TimeOnly currentTime)
    {
        return FindRoute(from, to, RouteSegmentWeigherFactory.Invoke(currentTime));
    }

    private Task<IEnumerable<RouteSegment>> FindRoute(RouteStop from, RouteStop to, IRouteSegmentWeigher weighter)
    {
        if (from.Equals(to)) return null;

        var visitedSegments = new HashSet<RouteSegment>();
        var queue = new PriorityQueue<(RouteStop RouteStop, RouteSegment? PreviousInSegment), ulong>(new CostComparer());
        var satisfiedRouteSegments = new PriorityQueue<RouteSegment, ulong>(new CostComparer());
        var childToParentDictionary = new Dictionary<RouteSegment, RouteSegment?>();

        queue.Enqueue(new(from, null), 0);
        while (queue.TryDequeue(out var pair, out var cost))
        {
            var nextSegments = pair.RouteStop.OutRouteSegments;
            var prevoiusSegment = pair.PreviousInSegment;
            foreach (var item in nextSegments)
            {
                childToParentDictionary.TryAdd(item, prevoiusSegment);
                if (!visitedSegments.Add(item)) continue;

                var costOfTravel = weighter.MeasureCost(item, prevoiusSegment, cost);

                if (item.To.Equals(to))
                {
                    satisfiedRouteSegments.Enqueue(item, costOfTravel);
                    continue; // we found enter to target. no reason to search from target
                }

                queue.Enqueue(new(item.To, item), costOfTravel);
            }
        }

        var path = new Queue<RouteSegment>();

        while (satisfiedRouteSegments.TryDequeue(out var segment, out var cost))
        {
            path.Enqueue(segment);

            while (childToParentDictionary.TryGetValue(segment, out var parentSegment))
            {
                if (parentSegment is null) break;
                segment = parentSegment;
                path.Enqueue(segment);
            }
            return Task.FromResult(path.Reverse());
        }

        return null;

    }
    
    private class CostComparer : IComparer<ulong>
    {
        public int Compare(ulong x, ulong y)
        {
            if (x == y) return 0;
            return x > y ? 1 : -1;
        }
    }
}
