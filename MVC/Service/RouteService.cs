using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Service;

using Route = BusManagementSystem.Models.Route;

public class RouteService : IRouteServiceInterface
{
    private readonly BusContext _context;
    private readonly ILoopServiceInterface _loopServiceInterface;

    public RouteService(BusContext context, ILoopServiceInterface loopServiceInterface)
    {
        _context = context;
        _loopServiceInterface = loopServiceInterface;
    }

    public List<Route> GetRoutes()
    {
        return _context.Routes.ToList();
    }

    public async Task<Route> GetRoute(int id)
    {
        var selectedRoute = await _context.Routes.FindAsync(id);
        if (selectedRoute == null)
        {
            throw new Exception("Route not found");
        }

        return selectedRoute;
    }

    public async Task<int> AddRoute(Route route)
    {
        int count = GetRoutesWithLoopId(route.LoopId).Count;
        route.Order = count + 1;
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        return route.Id;
    }

    public async Task<Route> UpdateRoute(Route route)
    {
        var selectedRoute = await _context.Routes.FindAsync(route.Id);
        if (selectedRoute == null)
        {
            throw new Exception("Route not found");
        }

        selectedRoute.Loop = route.Loop;
        selectedRoute.Stop = route.Stop;
        selectedRoute.Order = route.Order;

        _context.Entry(selectedRoute).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return selectedRoute;
    }

    public async Task<Route> DeleteRoute(int id)
    {
        var selectedRoute = await _context.Routes.FindAsync(id);
        if (selectedRoute == null)
        {
            throw new Exception("Route not found");
        }

        _context.Routes.RemoveRange(selectedRoute);
        await _context.SaveChangesAsync();

        return selectedRoute;
    }

    public async Task RemoveRoutesByLoopId(int loopId)
    {
        var routesToRemove = _context.Routes.Where(r => r.Loop.Id == loopId);
        _context.Routes.RemoveRange(routesToRemove);
        await _context.SaveChangesAsync();
    }

    public List<Loop> GetLoops()
    {
        return _context.Loops.ToList();
    }
    
    public async Task<Loop> GetLoop(int id)
    {
        return await _loopServiceInterface.GetLoop(id);
    }
    
    public List<Stop> GetStops()
    {
        return _context.Stops.ToList();
    }
    
    public List<Route> GetRoutesWithLoopId(int loopId)
    {
        return _context.Routes.Where(r => r.LoopId == loopId).ToList();
    }
    
    public async Task MoveRouteUp(int routeId)
    {
        var routeToMove = await _context.Routes.FindAsync(routeId);
        if (routeToMove == null)
        {
            throw new Exception("Route not found");
        }

        var previousRoute = await _context.Routes
            .Where(r => r.LoopId == routeToMove.LoopId && r.Order < routeToMove.Order)
            .OrderByDescending(r => r.Order)
            .FirstOrDefaultAsync();

        if (previousRoute != null)
        {
            int tempOrder = routeToMove.Order;
            routeToMove.Order = previousRoute.Order;
            previousRoute.Order = tempOrder;

            await _context.SaveChangesAsync();
        }
    }

    public async Task MoveRouteDown(int routeId)
    {
        var routeToMove = await _context.Routes.FindAsync(routeId);
        if (routeToMove == null)
        {
            throw new Exception("Route not found");
        }

        var nextRoute = await _context.Routes
            .Where(r => r.LoopId == routeToMove.LoopId && r.Order > routeToMove.Order)
            .OrderBy(r => r.Order)
            .FirstOrDefaultAsync();

        if (nextRoute != null)
        {
            int tempOrder = routeToMove.Order;
            routeToMove.Order = nextRoute.Order;
            nextRoute.Order = tempOrder;

            await _context.SaveChangesAsync();
        }
    }
}