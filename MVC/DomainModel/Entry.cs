using DomainModel;

namespace BusManagementSystem.DomainModel;

public class Entry
{
    public int entryId { get; set; }
    public DateTime timestamp { get; set; }
    public int boarded { get; set; }
    public int leftBehind { get; set; }
    public Driver driver { get; set; }
    public Bus bus { get; set; }
    public Loop loop { get; set; }
    public Stop stop { get; set; }
}