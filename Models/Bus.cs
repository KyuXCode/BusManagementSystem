namespace BusManagementSystem.Models;

public class Bus
{
    public int Id { get; set; }
    public int BusNumber { get; set; }

    public ICollection<Entry> Entries { get; }
}