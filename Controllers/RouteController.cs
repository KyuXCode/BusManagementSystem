using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Route = BusManagementSystem.Models.Route;

namespace BusManagementSystem.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class RouteController : Controller
{
    private readonly IRouteServiceInterface _routeServiceInterface;
    private readonly ILogger<RouteController> _logger;

    public RouteController(IRouteServiceInterface routeServiceInterface, ILogger<RouteController> logger)
    {
        _routeServiceInterface = routeServiceInterface;
        _logger = logger;
    }

    [Route("Route/loop/{loopId}")]
    public IActionResult Index(int loopId)
    {
        var routes = _routeServiceInterface.GetRoutes().Where(r => r.LoopId == loopId);
        return View(routes);
    }

    [Route("Route/SelectLoop")]
    public IActionResult SelectLoop()
    {
        var loops = _routeServiceInterface.GetLoops();
        return View(loops);
    }

    public IActionResult Create()
    {
        ViewBag.Loops = _routeServiceInterface.GetLoops();
        ViewBag.Stops = _routeServiceInterface.GetStops();
        return View();
    }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Create([Bind("Id, Order, StopId, LoopId")] Route route)
    // {
    //     _logger.LogInformation("Order: {Order}, StopId: {StopId}", route.Order, route.StopId);
    //     try
    //     {
    //         await _routeServiceInterface.AddRoute(route);
    //         _logger.LogInformation("Added route with id {id} at {time}", route.Id, DateTime.Now);
    //         return Redirect($"/Route/loop/{route.LoopId}");
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError("Create failed with exception {exception} at {time}", e.Message, DateTime.Now);
    //         throw;
    //     }
    // }
    
    [HttpPost("Route/loop/{loopId}/Create/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id, Order, StopId, LoopId")] Route route, int loopId)
    {
        _logger.LogInformation("Order: {Order}, StopId: {StopId}", route.Order, route.StopId);
        try
        {
            await _routeServiceInterface.AddRoute(route);
            _logger.LogInformation("Added route with id {id} at {time}", route.Id, DateTime.Now);
            return Redirect($"/Route/loop/{loopId}");
        }
        catch (Exception e)
        {
            _logger.LogError("Create failed with exception {exception} at {time}", e.Message, DateTime.Now);
            throw;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Order, Stop.StopId, Loop.LoopId")] Route route)
    {
        if (id != route.Id)
        {
            _logger.LogWarning("Route with id {routeId} not found at {time}.", route.Id, DateTime.Now);
            return NotFound();
        }


        try
        {
            await _routeServiceInterface.UpdateRoute(route);
            _logger.LogInformation("Edited route with id {id} at {time}", route.Id, DateTime.Now);
            return Redirect($"/Route/loop/{route.LoopId}");
        }
        catch (Exception e)
        {
            _logger.LogError("Edit Failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int routeId)
    {
        var route = await _routeServiceInterface.GetRoute(routeId);
        try
        {
            await _routeServiceInterface.DeleteRoutes(routeId);
            _logger.LogInformation("Deleted route with id {routeId} at {time}", routeId, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError("Delete route failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }

        return Redirect($"/Route/loop/{route.LoopId}");
    }
}