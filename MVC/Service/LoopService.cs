using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Route = BusManagementSystem.Models.Route;

namespace BusManagementSystem.Service;

public class LoopService : ILoopServiceInterface
{
    private readonly BusContext _context;

    public LoopService(BusContext context)
    {
        _context = context;
    }

    public List<Loop> GetLoops()
    {
        return _context.Loops.ToList();
    }

    public async Task<Loop> GetLoop(int id)
    {
        var selectedLoop = await _context.Loops.FindAsync(id);
        if (selectedLoop == null)
        {
            throw new Exception("Entry not found");
        }

        return selectedLoop;
    }

    public async Task<int> AddLoop(Loop loop)
    {
        _context.Loops.Add(loop);
        await _context.SaveChangesAsync();
        return loop.Id;
    }

    public async Task<Loop> UpdateLoop(Loop loop)
    {
        var selectedLoop = await _context.Loops.FindAsync(loop.Id);
        if (selectedLoop == null)
        {
            throw new Exception("Entry not found");
        }

        selectedLoop.Name = loop.Name;
        _context.Entry(selectedLoop).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return selectedLoop;
    }

    public async Task<Loop> DeleteLoop(int id)
    {
        var selectedLoop = await _context.Loops.FindAsync(id);
        if (selectedLoop == null)
        {
            throw new Exception("Entry not found");
        }
        
        _context.Loops.RemoveRange(selectedLoop);
        await _context.SaveChangesAsync();
        return selectedLoop;
    }

    public async Task<Loop> AddLoopWithRoutes(Loop loop, List<Route> routes)
    {
        loop.Routes = routes;
        _context.Loops.Add(loop);
        await _context.SaveChangesAsync();
        return loop;
    }
}