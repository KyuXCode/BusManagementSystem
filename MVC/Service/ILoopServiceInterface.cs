using BusManagementSystem.Models;
using Route = BusManagementSystem.Models.Route;
using Loop = BusManagementSystem.Models.Loop;

namespace BusManagementSystem.Service;

public interface ILoopServiceInterface
{
    List<Loop> GetLoops();
    Task<Loop> GetLoop(int id);
    Task<int> AddLoop(Loop loop);
    Task<Loop> UpdateLoop(Loop loop);
    Task<Loop> DeleteLoop(int id);
    Task<Loop> AddLoopWithRoutes(Loop loop, List<Route> routes);
    
}