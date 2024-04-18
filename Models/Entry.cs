namespace BusManagementSystem.Models;

public class Entry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int Boarded { get; set; }
    public int LeftBehind { get; set; }
    
    public string DriverId { get; set; }
    public int BusId { get; set; }
    public int LoopId { get; set; }
    public int StopId { get; set; }
    public Driver Driver { get; set; }
    public Bus Bus { get; set; }
    public Loop Loop { get; set; }
    public Stop Stop { get; set; }
}