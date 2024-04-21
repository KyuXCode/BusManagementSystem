using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Service;

public class DriverService : IDriverServiceInterface
{
    private readonly BusContext _context;

    public DriverService(BusContext context)
    {
        _context = context;
    }

    public List<Driver> GetDrivers()
    {
        return _context.Drivers.ToList();
    }

    public async Task<Driver> GetDriver(int id)
    {
        return await _context.Drivers.FindAsync(id);
    }

    public async Task<string> AddDriver(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return driver.Id;
    }

    public async Task<Driver> UpdateDriver(Driver driver)
    {
        var foundDriver = await _context.Drivers.FindAsync(driver.Id);
        if (foundDriver == null)
        {
            throw new Exception("Driver not found");
        }

        foundDriver.FirstName = driver.FirstName;
        foundDriver.LastName = driver.LastName;
        foundDriver.IsActive = driver.IsActive;
        

        // Mark the found driver as modified
        _context.Entry(foundDriver).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return foundDriver;
    }

    public async Task<Driver> DeleteDriver(int id)
    {
        var driverToDelete = await _context.Drivers.FindAsync(id);
        if (driverToDelete == null)
        {
            throw new Exception($"Driver withId {id} not found");
        }

        _context.Drivers.RemoveRange(driverToDelete);
        await _context.SaveChangesAsync();
        return driverToDelete;
    }
}