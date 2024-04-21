using BusManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Service;

public class EntryService : IEntryServiceInterface
{
    private readonly BusContext _context;

    public EntryService(BusContext context)
    {
        _context = context;
    }

    public async Task AddEntryAsync(Entry entry)
    {
        _context.Entries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Entry>> GetEntries()
    {
        return await _context.Entries
            .Include(e => e.Bus)
            .Include(e => e.Driver)
            .Include(e => e.Loop)
            .Include(e => e.Stop)
            .ToListAsync();
    }

    public async Task<Entry> GetEntry(int id)
    {
        var selectedEntry = await _context.Entries.FindAsync(id);
        if (selectedEntry == null)
        {
            throw new Exception("Entry not found");
        }

        return selectedEntry;
    }

    public async Task<int> AddEntry(Entry entry)
    {
        _context.Entries.Add(entry);
        await _context.SaveChangesAsync();
        return entry.Id;
    }

    public async Task<Entry> UpdateEntry(Entry entry)
    {
        var selectedEntry = await _context.Entries.FindAsync(entry.Id);
        if (selectedEntry == null)
        {
            throw new Exception("Entry not found");
        }

        selectedEntry.Boarded = entry.Boarded;
        selectedEntry.LeftBehind = entry.LeftBehind;
        selectedEntry.Bus = entry.Bus;
        selectedEntry.Driver = entry.Driver;
        selectedEntry.Loop = entry.Loop;
        selectedEntry.Stop = entry.Stop;
        selectedEntry.Timestamp = entry.Timestamp;

        await _context.SaveChangesAsync();
        return selectedEntry;
    }

    public async Task<Entry> DeleteEntry(int id)
    {
        var selectedEntry = await _context.Entries.FindAsync(id);
        if (selectedEntry == null)
        {
            throw new Exception("Entry not found");
        }

        _context.Entries.RemoveRange();
        await _context.SaveChangesAsync();
        return selectedEntry;
    }
    
    public async Task<List<Entry>> DeleteEntries(List<int> ids)
    {
        var entriesToDelete = await _context.Entries.Where(e => ids.Contains(e.Id)).ToListAsync();
        if (entriesToDelete.Count == 0)
        {
            throw new Exception("Entries not found");
        }

        _context.Entries.RemoveRange(entriesToDelete);
        await _context.SaveChangesAsync();
        return entriesToDelete;
    }
}