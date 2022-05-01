//namespace RouteFinder.Data.Models;

///// <summary>
///// откуда. куда. путь[]. транспорт[]. ключевые точки?
///// </summary>
//public record Route
//{
//    public RouteStop From { get; set; } = default!;
//    public RouteStop To { get; set; } = default!;

//    public List<RouteSegment> RouteSegments { get; init; } = new();

//    /// <summary>
//    /// Next route stop
//    /// </summary>
//    public IEnumerable<RouteStop> RouteStops => 
//        RouteSegments
//        .Select(s => s.To)
//        .Distinct();
//}
