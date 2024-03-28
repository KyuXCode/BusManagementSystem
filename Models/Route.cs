namespace BusManagementSystem.Models;

public class Route
{
    public int RouteId { get; set; }
    public int Order { get; set; }
    public Stop Stop { get; set; }
    public Loop Loop { get; set; }
}