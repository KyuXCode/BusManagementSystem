using BusManagementSystem.DomainModel;

namespace DomainModel;

public class Route
{
    public int routeId { get; set; }
    public int order { get; set; }
    public Stop stop { get; set; }
    public Loop loop { get; set; }
}