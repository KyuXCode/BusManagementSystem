namespace BusManagementSystem.Models;

public class Loop
{
    public int LoopId { get; set; }
    public string Name { get; set; }
    public List<Route> Routes { get; set; } = new List<Route>();
}   