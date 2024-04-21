using BusManagementSystem.Models;

namespace BusManagementSystem.Service;

public interface IDriverServiceInterface
{
    List<Driver> GetDrivers();
    Task<Driver> GetDriver(int id);
    Task<string> AddDriver(Driver driver);
    Task<Driver> UpdateDriver(Driver driver);
    Task<Driver> DeleteDriver(int id);
}