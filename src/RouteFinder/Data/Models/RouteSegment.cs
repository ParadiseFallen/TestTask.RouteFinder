namespace RouteFinder.Data.Models;

/// <summary>
/// (Graph edge) +
/// </summary>
public record RouteSegment
{

    #region Properties

    /// <summary>
    /// Segment starts at route stop
    /// </summary>
    public RouteStop From { get; init; }

    /// <summary>
    /// Segment ends at route stop
    /// </summary>
    public RouteStop To { get; init; }
    
    /// <summary>
    /// Segment related to transport
    /// </summary>
    public Transport Transport { get; init; }

    /// <summary>
    /// Time that spend by transport on this segment
    /// </summary>
    public TimeOnly TravelTime { get; init; }

    #endregion

    public RouteSegment(
        Transport transport,
        RouteStop start,
        RouteStop end,
        TimeOnly travelTime) =>
        (Transport,From,To,TravelTime) = (transport,start,end,travelTime);
}