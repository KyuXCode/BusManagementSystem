using BusManagementSystem.Models;

namespace BusManagementSystem.Service;

public interface IBusServiceInterface
{
    List<Bus> GetBuses();
    Task<Bus> GetBus(int id);
    Task<int> AddBus(Bus bus);
    Task<Bus> UpdateBus(Bus bus);
    Task<Bus> DeleteBus(int id);
}