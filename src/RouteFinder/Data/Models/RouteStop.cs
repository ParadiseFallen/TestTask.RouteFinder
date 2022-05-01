namespace RouteFinder.Data.Models;

/// <summary>
/// (Graph node)
/// </summary>
public record RouteStop
{
    #region Properties

    public int Id { get; init; }

    /// <summary>
    /// Avaliable buses on this stop
    /// </summary>
    
    
    ///// <summary>
    ///// This stop belongs to routes
    ///// </summary>
    //public HashSet<Route> Routes { get; init; } = new();

    /// <summary>
    /// RouteSegments from this stop to next stop
    /// </summary>
    public HashSet<RouteSegment> RouteSegments { get; init; } = new();

    #endregion

    #region Computed properties

    #region Route segments

    public IEnumerable<RouteSegment> InRouteSegments => 
        RouteSegments.Where(x => x.To == this);

    public IEnumerable<RouteSegment> OutRouteSegments =>
        RouteSegments.Where(x => x.From == this);

    #endregion

    #region Transports

    public IEnumerable<Transport> Transports =>
        RouteSegments.Select(x => x.Transport);

    public IEnumerable<Transport> InTransports =>
       InRouteSegments.Select(x => x.Transport);

    public IEnumerable<Transport> OutTransports =>
       OutRouteSegments.Select(x => x.Transport);

    #endregion

    #endregion
}
