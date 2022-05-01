using RouteFinder.Data.Models;

namespace RouteFinder.Abstractions.Services.Routing;
public interface IRouteFinder
{
    Task<IEnumerable<RouteSegment>> FindRoute(RouteStop from, RouteStop to, TimeOnly currentTime);
}