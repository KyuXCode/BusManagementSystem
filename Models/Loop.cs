namespace BusManagementSystem.Models;

public class Loop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Route> Routes { get; set; }
    public ICollection<Entry> Entries { get; }
    public ICollection<Stop> Stops { get; }
}