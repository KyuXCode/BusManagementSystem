namespace BusManagementSystem.Models;

public class Stop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public ICollection<Entry> Entries { get; }
    public ICollection<Stop> Stops { get; }
    public ICollection<Route> Routes { get; }

}