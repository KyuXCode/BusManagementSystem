using BusManagementSystem.Models;

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

    public List<Entry> GetEntries()
    {
        return _context.Entries.ToList();
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
}