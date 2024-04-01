using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BusManagementSystem.Models;

public class BusContext : IdentityDbContext<Driver>
{
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Loop> Loops { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Stop> Stops { get; set; }

    public string DbPath { get; }

    public BusContext(DbContextOptions<BusContext> options) : base(options)
    {
        
    }
}