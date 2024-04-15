using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Service;

public class BusService : IBusServiceInterface
{
    private readonly BusContext _context;

    public BusService(BusContext context)
    {
        _context = context;
    }

    public List<Bus> GetBuses()
    {
        return _context.Buses.ToList();
    }

    public async Task<Bus> GetBus(int id)
    {
        var selectedBus = await _context.Buses.FindAsync(id);
        if (selectedBus == null)
        {
            throw new Exception("Entry not found");
        }

        return selectedBus;
    }

    public async Task<int> AddBus(Bus bus)
    {
        _context.Buses.Add(bus);
        await _context.SaveChangesAsync();
        return bus.Id;
    }

    public async Task<Bus> UpdateBus(Bus bus)
    {
        var selectedBus = await _context.Buses.FindAsync(bus.Id);
        if (selectedBus == null)
        {
            throw new Exception("Bus not found");
        }

        selectedBus.BusNumber = bus.BusNumber;

        // Mark the found bus as modified
        _context.Entry(selectedBus).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return selectedBus;
    }

    public async Task<Bus> DeleteBus(int id)
    {
        var busToDelete = await _context.Buses.FindAsync(id);
        if (busToDelete == null)
        {
            throw new Exception($"Bus with id: {id} not found");
        }

        //Using RemoveRange in case adding feature for deleting multiple at the same time
        _context.Buses.RemoveRange(busToDelete);
        await _context.SaveChangesAsync();
        return busToDelete;
    }
}