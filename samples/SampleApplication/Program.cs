using RouteFinder.Abstractions.Services.Routing;
using RouteFinder.Data.Models;
using RouteFinder.Extensions;
using RouteFinder.Services.Routing;
using RouteFinder.Services.Serialization;

bool isExited = false;
var deserializer = new TxtRouteNetworkDeserializer();
RouteNetwork? routeNetwork = null;

do
{
    if (routeNetwork is null)
    {
        Console.WriteLine("No loaded route network");
        var pathToDataFile = RequestPathDataFile();
        using var fileStream = File.OpenRead(pathToDataFile);
        routeNetwork = await deserializer.Deserialize(fileStream);
    }
    //// Раскоментить если хотите смену файла  рантайме без перезапуска
    //else
    //{
    //    Console.Write("Do you want to load another file? [Y/N] : ");
    //    if (Console.ReadLine()?.Equals("Y") ?? false)
    //    {
    //        routeNetwork = null;
    //        continue;
    //    }
    //}

    PrintAvaliableRoutes(routeNetwork);

    var startRouteStop = RequestRouteStop(routeNetwork, "Enter current route stop (id): ");
    var targetRouteStop = RequestRouteStop(routeNetwork, "Enter target route stop (id): ");
    var currentTime = RequestCurrentTime();
    var pathfinder = new DefaultRoutePathfinder(GetWeighterFactory());

    if (startRouteStop.Equals(targetRouteStop))
    {
        Console.WriteLine("Start == Target");
        Console.ReadKey();
        continue;
    }

    var path = await pathfinder.FindRoute(startRouteStop, targetRouteStop, currentTime);

    if (path is null)
    {
        Console.WriteLine("No paths was found");
        Console.ReadKey();
        continue;
    }

    PrintResult(path,currentTime);

    Console.WriteLine();
    Console.WriteLine("Press any key");
    Console.ReadKey();

} while (!isExited);


static void PrintResult(IEnumerable<RouteSegment> routeSegments,TimeOnly startTime)
{
    var firstSegment = routeSegments.First();
    var totalPrice = routeSegments.Select(x => x.Transport).DistinctBy(x => x.Id).Select(x => x.Price).Aggregate((prev, current) => prev + current);

   

    Console.WriteLine();

    Console.WriteLine($"Total cost : {totalPrice} RUB");


    Console.WriteLine($"Total time : {routeSegments.GetTotalTime(startTime)}");

    Console.WriteLine();

    Console.WriteLine($"First transport : {firstSegment.Transport.Id}");
    foreach (var segment in routeSegments.ShiftAndZipWithSelf())
    {
        Console.WriteLine($"{segment.First.From.Id} ==({segment.First.TravelTime})=> {segment.First.To.Id}");
        if (!segment.First.Transport.Equals(segment.Second.Transport) && !segment.Second.Equals(firstSegment))
            Console.WriteLine($"Change transport to : {segment.Second.Transport.Id}");
    }

    Console.WriteLine();
}

static Func<TimeOnly,IRouteSegmentWeigher> GetWeighterFactory()
{
    Console.WriteLine("Strategies");
    Console.WriteLine("1: Cheap");
    Console.WriteLine("2: Fast");
    Console.Write("Choose strategy:");
    var isCheapMode = int.Parse(Console.ReadLine()) == 1;
    return isCheapMode
        ? (time) => new CostRespectWeighter()
        : (time) => new TimeRespectWeighter(time);
}

static string RequestPathDataFile()
{
    do
    {
        Console.Write("Enter data file path : ");
        var path = Console.ReadLine();
        if (path is not null && File.Exists(path))
            return path;
        Console.WriteLine("You enter wrong path");
    } while (true);
}

static TimeOnly RequestCurrentTime()
{
    Console.Write("Enter current time (format {10:00} ): ");
    return TimeOnly.Parse(Console.ReadLine());
}

static RouteStop RequestRouteStop(RouteNetwork routeNetwork, string message)
{
    Console.Write(message);
    var routeStopId = int.Parse(Console.ReadLine());
    return routeNetwork.RouteStops.First(x => x.Id == routeStopId);
}

static void PrintAvaliableRoutes(RouteNetwork routeNetwork)
{
    var transports = routeNetwork.Transports;
    Console.WriteLine("Discovered avaliable routes : ");
    Console.WriteLine("* - route intersects");
    Console.WriteLine("\n");
    foreach (var transport in transports)
    {
        Console.WriteLine($"Transport : [{transport.Id}] | Start at route stop : [{transport.StartRouteStop.Id}] | Start at : [{transport.StartTime}] | Price : [{transport.Price}]");
        var firstSegment = transport.RouteSegments.First();
        Console.Write($"{firstSegment.From.Id}{(firstSegment.From.OutRouteSegments.Count() > 1 ? "*" : null)}");
        foreach (var item in transport.RouteSegments)
            Console.Write($" ==({item.TravelTime})=> {item.To.Id}{(item.To.OutRouteSegments.Count() > 1 ? "*" : null)} ");

        Console.WriteLine("\n");
    }
}