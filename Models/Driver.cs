using Microsoft.AspNetCore.Identity;

namespace BusManagementSystem.Models;

public class Driver : IdentityUser
{
    public int DriverId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsManager { get; set; }
    public bool IsActive { get; set; }
}