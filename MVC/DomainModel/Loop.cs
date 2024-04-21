using Route = DomainModel.Route;

namespace BusManagementSystem.DomainModel;

public class Loop
{
    public int loopId { get; set; }
    public string name { get; set; }
    public List<Route> routes { get; set; } = new List<Route>();
}   