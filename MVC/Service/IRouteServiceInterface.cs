using BusManagementSystem.Models;

namespace BusManagementSystem.Service;

using Route = BusManagementSystem.Models.Route;

public interface IRouteServiceInterface
{
    List<Route> GetRoutes();
    Task<Route> GetRoute(int id);
    Task<int> AddRoute(Route route);
    Task<Route> UpdateRoute(Route route);
    Task<Route> DeleteRoute(int id);
    Task RemoveRoutesByLoopId(int loopId);
    List<Loop> GetLoops();
    Task<Loop> GetLoop(int id);
    List<Stop> GetStops();
    List<Route> GetRoutesWithLoopId(int loopId);

    Task MoveRouteUp(int routeId);
    Task MoveRouteDown(int routeId);
}