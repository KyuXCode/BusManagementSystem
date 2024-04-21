using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Service;

public class StopService : IStopServiceInterface
{
    private readonly BusContext _context;

    public StopService(BusContext context)
    {
        _context = context;
    }

    public List<Stop> GetStops()
    {
        return _context.Stops.ToList();
    }

    public async Task<Stop> GetStop(int id)
    {
        var selectedStop = await _context.Stops.FindAsync(id);
        if (selectedStop == null)
        {
            throw new Exception("Stop not found");
        }

        return selectedStop;
    }

    public async Task<int> AddStop(Stop stop)
    {
        _context.Stops.Add(stop);
        await _context.SaveChangesAsync();
        return stop.Id;
    }

    public async Task<Stop> UpdateStop(Stop stop)
    {
        var selectedStop = await _context.Stops.FindAsync(stop.Id);
        if (selectedStop == null)
        {
            throw new Exception("Stop not found");
        }

        selectedStop.Name = stop.Name;
        selectedStop.Longitude = stop.Longitude;
        selectedStop.Latitude = stop.Latitude;

        _context.Entry(selectedStop).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return selectedStop;
    }

    public async Task<Stop> DeleteStop(int id)
    {
        var stopToDelete = await _context.Stops.FindAsync(id);
        if (stopToDelete == null)
        {
            throw new Exception($"Stop with id:{id} not found");
        }

        _context.Stops.RemoveRange(stopToDelete);
        await _context.SaveChangesAsync();
        return stopToDelete;
    }
}