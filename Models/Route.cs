namespace BusManagementSystem.Models;

public class Route
{
    public int Id { get; set; }
    public int Order { get; set; }
    
    public int StopId { get; set; }
    public int LoopId { get; set; }
    public Stop Stop { get; set; }
    public Loop Loop { get; set; }
}