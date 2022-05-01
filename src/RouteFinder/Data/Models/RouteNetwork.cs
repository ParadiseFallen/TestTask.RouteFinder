namespace RouteFinder.Data.Models;

/// <summary>
/// +
/// </summary>
public record RouteNetwork
{
    public HashSet<RouteStop> RouteStops { get; init; } = new();
    public HashSet<Transport> Transports { get; init; } = new();
}
