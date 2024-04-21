using BusManagementSystem.Models;

namespace BusManagementSystem.Service;

public interface IEntryServiceInterface
{
    Task AddEntryAsync(Entry entry);
    Task SaveChangesAsync();
    Task<List<Entry>> GetEntries();
    Task<Entry> GetEntry(int id);
    Task<int> AddEntry(Entry entry);
    Task<Entry> UpdateEntry(Entry entry);
    Task<Entry> DeleteEntry(int id);
}