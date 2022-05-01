using RouteFinder.Data.Models;

namespace RouteFinder.Abstractions.Services.Serialization;
public interface IRouteNetworkDeserializer
{
    Task<RouteNetwork> Deserialize(Stream stream);
}
