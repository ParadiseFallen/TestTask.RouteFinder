using RouteFinder.Data.Models;

namespace RouteFinder.Abstractions.Services.Routing;
public interface IRouteSegmentWeigher
{
    public ulong MeasureCost(RouteSegment currentSegment, RouteSegment? previousSegment = null, ulong currentCost = 0ul);
}
