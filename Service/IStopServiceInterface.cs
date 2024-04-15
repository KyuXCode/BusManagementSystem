using BusManagementSystem.Models;

namespace BusManagementSystem.Service;

public interface IStopServiceInterface
{
    List<Stop> GetStops();
    Task<Stop> GetStop(int id);
    Task<int> AddStop(Stop bus);
    Task<Stop> UpdateStop(Stop bus);
    Task<Stop> DeleteStop(int id);
}