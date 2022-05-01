namespace RouteFinder.Data.Models;

/// <summary>
/// +
/// </summary>
public record Transport
{
    #region Properties

    public int Id { get; init; }
    public uint Price { get; set; }
    public RouteStop StartRouteStop { get; set; } = default!;
    public HashSet<RouteSegment> RouteSegments { get; set; } = new();
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; } = new TimeOnly(0, 0);

    #endregion

    #region Computed properties

    public TimeOnly LoopTime => 
        RouteSegments
        .Select(x => x.TravelTime)
        .Aggregate((prev,current) => prev.Add(current.ToTimeSpan()));

    #endregion
}
