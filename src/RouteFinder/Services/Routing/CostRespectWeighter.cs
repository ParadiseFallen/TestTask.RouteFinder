using RouteFinder.Abstractions.Services.Routing;
using RouteFinder.Data.Models;

namespace RouteFinder.Services.Routing;
public class CostRespectWeighter : IRouteSegmentWeigher
{

    public ulong MeasureCost(
        RouteSegment currentSegment, 
        RouteSegment? previousSegment = null, 
        ulong currentCost = 0) =>
         currentCost + 1ul +
            // if change transport increase total cost
            (previousSegment?.Transport.Equals(currentSegment.Transport) ?? true 
            ? 0ul 
            : currentSegment.Transport.Price * 2);

    
}
