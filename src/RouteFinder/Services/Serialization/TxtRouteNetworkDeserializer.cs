using RouteFinder.Abstractions.Services.Serialization;
using RouteFinder.Data.Models;

namespace RouteFinder.Services.Serialization;
public class TxtRouteNetworkDeserializer : IRouteNetworkDeserializer
{
    /*
     * File structure:
     * [0] : NumberOfTransports     | 2
     * [1] : NumberOfStops          | 4
     * [2] : TransportStartTime[]   | 10:00 11:00
     * [3] : TransportCost[]        | 10 20   
     * [4..N] : Route[N]            | 2 (<= number of stops) 1 (<=stop with id 1) 2 (<= stop with id 2) 5 (<= time for moving from 1 to 3) 7 (<= time for moving from 3 to 1)
     *  
     *
     * Creation order:
     * 1. Create route stops
     * 2. Create transports
     * 3. Create route segments and bind thm to transport and route stops
     */
    public async Task<RouteNetwork> Deserialize(Stream dataStream)
    {
        var dataRows = await ConvertToDataRows(dataStream);

        var routeStops = await ParseRouteStops(dataRows);
        var transports = ParseTransports(routeStops, dataRows);

        var segments = routeStops.SelectMany(x => x.OutRouteSegments).OrderBy(x => x.Transport.Id);

        return new()
        {
            RouteStops = new(routeStops),
            Transports = new(transports)
        };
    }

    private static async Task<IEnumerable<string>> ConvertToDataRows(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        var content = await streamReader.ReadToEndAsync();
        return content
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x));
    }

    private static Task<IEnumerable<RouteStop>> ParseRouteStops(IEnumerable<string> dataRows)
    {
        var numberOfStops = int.Parse(dataRows.ElementAt(1));
        var resultRouteStopList = new List<RouteStop>();
        for (int i = 1; i <= numberOfStops; i++)
            resultRouteStopList.Add(new RouteStop()
            {
                Id = i,
            });

        return Task.FromResult(resultRouteStopList as IEnumerable<RouteStop>);
    }

    private static IEnumerable<Transport> ParseTransports(IEnumerable<RouteStop> routeStops, IEnumerable<string> dataRows)
    {
        var resultTransportList = new List<Transport>();
        var transportCount = int.Parse(dataRows.ElementAt(0));
        var startTimes = dataRows.ElementAt(2).Split(" ").Select(TimeOnly.Parse);
        var travelPrices = dataRows.ElementAt(3).Split(" ").Select(uint.Parse);

        for (int i = 0; i < transportCount; i++)
        {
            var routeDataRow = dataRows.ElementAt(4 + i);
            var transport = new Transport()
            {
                Id = i,
                StartTime = startTimes.ElementAt(i),
                Price = travelPrices.ElementAt(i),
            };

            transport.RouteSegments = new(ParseRouteSegment(routeStops, transport, routeDataRow));
            transport.StartRouteStop = transport.RouteSegments.First().From;


            resultTransportList.Add(transport);
        }

        return resultTransportList;

    }

    private static IEnumerable<RouteSegment> ParseRouteSegment(IEnumerable<RouteStop> routeStops, Transport transport, string dataRow)
    {
        var dataRowValues = dataRow.Split(" ");
        var numberOfStops = int.Parse(dataRowValues.ElementAt(0));

        var startStops = dataRowValues
            .Skip(1)
            .Take(numberOfStops)
            .Select(x => routeStops.First(s => s.Id == int.Parse(x)))
            .ToList();
        var endStops = ParseEndStops(new List<RouteStop>(startStops));

        var travelTimes = dataRowValues
            .Skip(1 + numberOfStops)
            .Select(x => new TimeOnly(0, int.Parse(x)));

        var resultRouteSegmentsList = new List<RouteSegment>();

        var chunkedRouteStops = startStops.Zip(endStops, (start, end) => (From: start, To: end)).Zip(travelTimes);
        foreach (var pair in chunkedRouteStops)
        {
            var (startRouteStop, endRouteStop) = pair.First;
            var routeSegment = new RouteSegment(transport, startRouteStop, endRouteStop, pair.Second);
            resultRouteSegmentsList.Add(routeSegment);
            startRouteStop.RouteSegments.Add(routeSegment);
            endRouteStop.RouteSegments.Add(routeSegment);
        }

        return resultRouteSegmentsList;
    }

    private static IEnumerable<RouteStop> ParseEndStops(IEnumerable<RouteStop> startStops)
    {
        var firstStop = startStops.First();
        var result = startStops.Skip(1).ToList();
        result.Add(firstStop);
        return result;
    }

}